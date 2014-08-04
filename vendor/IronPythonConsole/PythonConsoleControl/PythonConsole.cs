// Copyright (c) 2010 Joe Moorhouse

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Style = Microsoft.Scripting.Hosting.Shell.Style;

namespace Quickbeam.PythonConsoleControl
{
    public delegate void ConsoleInitializedEventHandler(object sender, EventArgs e);
    
    /// <summary>
    /// Custom IronPython console. The command dispacher runs on a separate UI thread from the REPL
    /// and also from the WPF control.
    /// </summary>
    public class PythonConsole : IConsole, IDisposable
    {
        bool _allowFullAutocompletion = true;
        public bool AllowFullAutocompletion
        {
            get { return _allowFullAutocompletion; }
            set { _allowFullAutocompletion = value; }
        }

        bool _disableAutocompletionForCallables = true;
        public bool DisableAutocompletionForCallables
        {
            get { return _disableAutocompletionForCallables; }
            set 
            {
                if (_textEditor.CompletionProvider != null) _textEditor.CompletionProvider.ExcludeCallables = value;
                _disableAutocompletionForCallables = value; 
            }
        }

        bool _allowCtrlSpaceAutocompletion;
        public bool AllowCtrlSpaceAutocompletion
        {
            get { return _allowCtrlSpaceAutocompletion; }
            set { _allowCtrlSpaceAutocompletion = value; }
        }

        readonly PythonTextEditor _textEditor;
        private const int LineReceivedEventIndex = 0; // The index into the waitHandles array where the lineReceivedEvent is stored.
        readonly ManualResetEvent _lineReceivedEvent = new ManualResetEvent(false);
        readonly ManualResetEvent _disposedEvent = new ManualResetEvent(false);
        readonly WaitHandle[] _waitHandles;
        int _promptLength = 4;
        readonly List<string> _previousLines = new List<string>();
        readonly CommandLine _commandLine;
        readonly CommandLineHistory _commandLineHistory = new CommandLineHistory();

        volatile bool _executing;

        // This is the thread upon which all commands execute unless the dipatcher is overridden.
        readonly Thread _dispatcherThread;
        Window _dispatcherWindow;
        Dispatcher _dispatcher;

        string _scriptText = String.Empty;
        bool _consoleInitialized;
        readonly string _prompt;
      
        public event ConsoleInitializedEventHandler ConsoleInitialized;

        public ScriptScope ScriptScope
        {
            get { return _commandLine.ScriptScope; }
        }

        public PythonConsole(PythonTextEditor textEditor, CommandLine commandLine)
        {   
            _waitHandles = new WaitHandle[] { _lineReceivedEvent, _disposedEvent };

            _commandLine = commandLine;
            _textEditor = textEditor;
            textEditor.CompletionProvider = new PythonConsoleCompletionDataProvider(commandLine) { ExcludeCallables = _disableAutocompletionForCallables };
            textEditor.PreviewKeyDown += textEditor_PreviewKeyDown;
            textEditor.TextEntering += textEditor_TextEntering;
            _dispatcherThread = new Thread(DispatcherThreadStartingPoint);
            _dispatcherThread.SetApartmentState(ApartmentState.STA);
            _dispatcherThread.IsBackground = true;
            _dispatcherThread.Start();

            // Only required when running outside REP loop.
            _prompt = ">>> ";

            // Set commands:
            _textEditor.textArea.Dispatcher.Invoke(new Action(() =>
            {
                CommandBinding pasteBinding = null;
                CommandBinding copyBinding = null;
                CommandBinding cutBinding = null;
                CommandBinding undoBinding = null;
                CommandBinding deleteBinding = null;
                foreach (CommandBinding commandBinding in (_textEditor.textArea.CommandBindings))
                {
                    if (commandBinding.Command == ApplicationCommands.Paste) pasteBinding = commandBinding;
                    if (commandBinding.Command == ApplicationCommands.Copy) copyBinding = commandBinding;
                    if (commandBinding.Command == ApplicationCommands.Cut) cutBinding = commandBinding;
                    if (commandBinding.Command == ApplicationCommands.Undo) undoBinding = commandBinding;
                    if (commandBinding.Command == ApplicationCommands.Delete) deleteBinding = commandBinding;
                }
                // Remove current bindings completely from control. These are static so modifying them will cause other
                // controls' behaviour to change too.
                if (pasteBinding != null) _textEditor.textArea.CommandBindings.Remove(pasteBinding);
                if (copyBinding != null) _textEditor.textArea.CommandBindings.Remove(copyBinding);
                if (cutBinding != null) _textEditor.textArea.CommandBindings.Remove(cutBinding);
                if (undoBinding != null) _textEditor.textArea.CommandBindings.Remove(undoBinding);
                if (deleteBinding != null) _textEditor.textArea.CommandBindings.Remove(deleteBinding);
                _textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPaste, CanPaste));
                _textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopy,
                    PythonEditingCommandHandler.CanCutOrCopy));
                _textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut,
                    PythonEditingCommandHandler.OnCut, CanCut));
                _textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, OnUndo, CanUndo));
                _textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete,
                    PythonEditingCommandHandler.OnDelete(ApplicationCommands.NotACommand), CanDeleteCommand));
            }));
            var codeContext = DefaultContext.Default;
            // Set dispatcher to run on a UI thread independent of both the Control UI thread and thread running the REPL.
            ClrModule.SetCommandDispatcher(codeContext, DispatchCommand);
        }

        protected void DispatchCommand(Delegate command)
        {
            if (command == null) return;
            // Slightly involved form to enable keyboard interrupt to work.
            _executing = true;
            var operation = _dispatcher.BeginInvoke(DispatcherPriority.Normal, command);
            while (_executing)
            {
                if (operation.Status != DispatcherOperationStatus.Completed) 
                    operation.Wait(TimeSpan.FromSeconds(1));
                if (operation.Status == DispatcherOperationStatus.Completed)
                    _executing = false;
            }
        }

        private void DispatcherThreadStartingPoint()
        {
            _dispatcherWindow = new Window();
            _dispatcher = _dispatcherWindow.Dispatcher;
            while (true)
            {
                try
                {
                    Dispatcher.Run();
                }
                catch (ThreadAbortException tae)
                {
                    if (tae.ExceptionState is KeyboardInterruptException)
                    {
                        Thread.ResetAbort();
                        _executing = false;
                    }
                }
            }
        }

        public void SetDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Dispose()
        {
            _disposedEvent.Set();
            _textEditor.PreviewKeyDown -= textEditor_PreviewKeyDown;
            _textEditor.TextEntering -= textEditor_TextEntering;
        }

        public TextWriter Output
        {
            get { return null; }
            set { }
        }

        public TextWriter ErrorOutput
        {
            get { return null; }
            set { }
        }

        #region CommandHandling
        protected void CanPaste(object target, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = !IsInReadOnlyRegion;
        }

        protected void CanCut(object target, CanExecuteRoutedEventArgs args)
        {
            if (!CanDelete)
            {
                args.CanExecute = false;
            }
            else
                PythonEditingCommandHandler.CanCutOrCopy(target, args);
        }

        protected void CanDeleteCommand(object target, CanExecuteRoutedEventArgs args)
        {
            if (!CanDelete)
            {
                args.CanExecute = false;
            }
            else
                PythonEditingCommandHandler.CanDelete(target, args);
        }

        protected void CanUndo(object target, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = false;
        }

        protected void OnPaste(object target, ExecutedRoutedEventArgs args)
        {
            if (target != _textEditor.textArea) return;
            var textArea = _textEditor.textArea;
            if (textArea == null || textArea.Document == null) return;
            Debug.WriteLine(Clipboard.GetText(TextDataFormat.Html));

            // convert text back to correct newlines for this document
            var newLine = TextUtilities.GetNewLineFromDocument(textArea.Document, textArea.Caret.Line);
            var text = TextUtilities.NormalizeNewLines(Clipboard.GetText(), newLine);
            var commands = text.Split(new[] {newLine}, StringSplitOptions.None);
            var scriptText = "";
            if (commands.Length > 1)
            {
                text = newLine;
                foreach (var command in commands)
                {
                    text += "... " + command + newLine;
                    scriptText += command.Replace("\t", "   ") + newLine;
                }
            }

            if (!string.IsNullOrEmpty(text))
            {
                var fullLine = textArea.Options.CutCopyWholeLine && Clipboard.ContainsData(LineSelectedType);
                var rectangular = Clipboard.ContainsData(RectangleSelection.RectangularSelectionDataType);
                if (fullLine)
                {
                    var currentLine = textArea.Document.GetLineByNumber(textArea.Caret.Line);
                    if (textArea.ReadOnlySectionProvider.CanInsert(currentLine.Offset))
                    {
                        textArea.Document.Insert(currentLine.Offset, text);
                    }
                }
                else if (rectangular && textArea.Selection.IsEmpty)
                {
                    if (!RectangleSelection.PerformRectangularPaste(textArea, textArea.Caret.Offset, text, false))
                        _textEditor.Write(text, false);
                }
                else
                {
                    _textEditor.Write(text, false);
                }
            }
            textArea.Caret.BringCaretToView();
            args.Handled = true;

            if (commands.Length <= 1) return;
            lock (_scriptText)
            {
                _scriptText = scriptText;
            }
            _dispatcherWindow.Dispatcher.BeginInvoke(new Action(ExecuteStatements));
        }

        protected void OnCopy(object target, ExecutedRoutedEventArgs args)
        {
            if (target != _textEditor.textArea) return;
            if (_textEditor.SelectionLength == 0 && _executing)
            {
                // Send the 'Ctrl-C' abort 
                //if (!IsInReadOnlyRegion)
                //{
                    MoveToHomePosition();
                    //textEditor.Column = GetLastTextEditorLine().Length + 1;
                    //textEditor.Write(Environment.NewLine);
                //}
                _dispatcherThread.Abort(new KeyboardInterruptException(""));
                args.Handled = true;
            }
            else PythonEditingCommandHandler.OnCopy(target, args);
        }

        const string LineSelectedType = "MSDEVLineSelect";  // This is the type VS 2003 and 2005 use for flagging a whole line copy

        protected void OnUndo(object target, ExecutedRoutedEventArgs args)
        {
        }
        #endregion

        /// <summary>
        /// Run externally provided statements in the Console Engine. 
        /// </summary>
        /// <param name="statements"></param>
        public void RunStatements(string statements)
        {
            MoveToHomePosition();
            lock (_scriptText)
            {
                _scriptText = statements;
            }
            _dispatcher.BeginInvoke(new Action(ExecuteStatements));
        }

        /// <summary>
        /// Run on the statement execution thread. 
        /// </summary>
        void ExecuteStatements()
        {
            lock (_scriptText)
            {
                _textEditor.Write("\r\n");
                var scriptSource = _commandLine.ScriptScope.Engine.CreateScriptSourceFromString(_scriptText, SourceCodeKind.Statements);
                var error = "";
                try
                {
                    _executing = true;
                    scriptSource.Execute(_commandLine.ScriptScope);
                }
                catch (ThreadAbortException tae)
                {
                    if (tae.ExceptionState is KeyboardInterruptException) Thread.ResetAbort();
                    error = "KeyboardInterrupt" + Environment.NewLine;
                }
                catch (SyntaxErrorException exception)
                {
                    var eo = _commandLine.ScriptScope.Engine.GetService<ExceptionOperations>();
                    error = eo.FormatException(exception);
                }
                catch (Exception exception)
                {
                    var eo = _commandLine.ScriptScope.Engine.GetService<ExceptionOperations>();
                    error = eo.FormatException(exception) + Environment.NewLine;
                }
                _executing = false;
                if (error != "") _textEditor.Write(error);
                _textEditor.Write(_prompt);
            }
        }

        /// <summary>
        /// Returns the next line typed in by the console user. If no line is available this method
        /// will block.
        /// </summary>
        public string ReadLine(int autoIndentSize)
        {
            var indent = String.Empty;
            if (autoIndentSize > 0)
            {
                indent = String.Empty.PadLeft(autoIndentSize);
                Write(indent, Style.Prompt);
            }

            var line = ReadLineFromTextEditor();
            if (line != null)
            {
                return indent + line;
            }
            return null;
        }

        /// <summary>
        /// Writes text to the console.
        /// </summary>
        public void Write(string text, Style style)
        {
            _textEditor.Write(text);
            if (style != Style.Prompt) return;
            _promptLength = text.Length;
            if (_consoleInitialized) return;
            _consoleInitialized = true;
            if (ConsoleInitialized != null) ConsoleInitialized(this, EventArgs.Empty);
        }

        /// <summary>
        /// Writes text followed by a newline to the console.
        /// </summary>
        public void WriteLine(string text, Style style)
        {
            Write(text + Environment.NewLine, style);
        }

        /// <summary>
        /// Writes an empty line to the console.
        /// </summary>
        public void WriteLine()
        {
            Write(Environment.NewLine, Style.Out);
        }

        /// <summary>
        /// Indicates whether there is a line already read by the console and waiting to be processed.
        /// </summary>
        public bool IsLineAvailable
        {
            get
            {
                lock (_previousLines)
                {
                    return _previousLines.Count > 0;
                }
            }
        }

        /// <summary>
        /// Gets the text that is yet to be processed from the console. This is the text that is being
        /// typed in by the user who has not yet pressed the enter key.
        /// </summary>
        public string GetCurrentLine()
        {
            string fullLine = GetLastTextEditorLine();
            return fullLine.Substring(_promptLength);
        }

        /// <summary>
        /// Gets the lines that have not been returned by the ReadLine method. This does not
        /// include the current line.
        /// </summary>
        public string[] GetUnreadLines()
        {
            return _previousLines.ToArray();
        }

        string GetLastTextEditorLine()
        {
            return _textEditor.GetLine(_textEditor.TotalLines - 1);
        }

        string ReadLineFromTextEditor()
        {
            int result = WaitHandle.WaitAny(_waitHandles);
            if (result == LineReceivedEventIndex)
            {
                lock (_previousLines)
                {
                    string line = _previousLines[0];
                    _previousLines.RemoveAt(0);
                    if (_previousLines.Count == 0)
                    {
                        _lineReceivedEvent.Reset();
                    }
                    return line;
                }
            }
            return null;
        }

        /// <summary>
        /// Processes characters entered into the text editor by the user.
        /// </summary>
        void textEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    if (!CanDelete) e.Handled = true;
                    return;
                case Key.Tab:
                    if (IsInReadOnlyRegion) e.Handled = true;
                    return;
                case Key.Back:
                    if (!CanBackspace) e.Handled = true;
                    return;
                case Key.Home:
                    MoveToHomePosition();
                    e.Handled = true;
                    return;
                case Key.Down:
                    if (!IsInReadOnlyRegion) MoveToNextCommandLine();
                    e.Handled = true;
                    return;
                case Key.Up:
                    if (!IsInReadOnlyRegion) MoveToPreviousCommandLine();
                    e.Handled = true;
                    return;
            }
        }

        /// <summary>
        /// Processes characters entering into the text editor by the user.
        /// </summary>
        void textEditor_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _textEditor.RequestCompletioninsertion(e);
                }
            }

            if (IsInReadOnlyRegion)
            {
                e.Handled = true;
            }
            else
            {
                if (e.Text[0] == '\n')
                {
                    OnEnterKeyPressed();
                }

                if (e.Text[0] == '.' && _allowFullAutocompletion)
                {
                    _textEditor.ShowCompletionWindow();
                }

                if ((e.Text[0] == ' ') && (Keyboard.Modifiers == ModifierKeys.Control))
                {
                    e.Handled = true;
                    if (_allowCtrlSpaceAutocompletion) _textEditor.ShowCompletionWindow();
                }
            }
        }

        /// <summary>
        /// Move cursor to the end of the line before retrieving the line.
        /// </summary>
        void OnEnterKeyPressed()
        {
            _textEditor.StopCompletion();
            if (_textEditor.WriteInProgress) return;
            lock (_previousLines)
            {
                // Move cursor to the end of the line.
                _textEditor.Column = GetLastTextEditorLine().Length + 1;

                // Append line.
                string currentLine = GetCurrentLine();
                _previousLines.Add(currentLine);
                _commandLineHistory.Add(currentLine);

                _lineReceivedEvent.Set();
            }
        }

        /// <summary>
        /// Returns true if the cursor is in a readonly text editor region.
        /// </summary>
        bool IsInReadOnlyRegion
        {
            get { return IsCurrentLineReadOnly || IsInPrompt; }
        }

        /// <summary>
        /// Only the last line in the text editor is not read only.
        /// </summary>
        bool IsCurrentLineReadOnly
        {
            get { return _textEditor.Line < _textEditor.TotalLines; }
        }

        /// <summary>
        /// Determines whether the current cursor position is in a prompt.
        /// </summary>
        bool IsInPrompt
        {
            get { return _textEditor.Column - _promptLength - 1 < 0; }
        }

        /// <summary>
        /// Returns true if the user can delete at the current cursor position.
        /// </summary>
        bool CanDelete
        {
            get
            {
                if (_textEditor.SelectionLength > 0) return SelectionIsDeletable;
                return !IsInReadOnlyRegion;
            }
        }

        /// <summary>
        /// Returns true if the user can backspace at the current cursor position.
        /// </summary>
        bool CanBackspace
        {
            get
            {
                if (_textEditor.SelectionLength > 0) return SelectionIsDeletable;
                var cursorIndex = _textEditor.Column - _promptLength - 1;
                return !IsCurrentLineReadOnly && (cursorIndex > 0 || (cursorIndex == 0 && _textEditor.SelectionLength > 0));
            }
        }

        bool SelectionIsDeletable
        {
            get
            {
                return (!_textEditor.SelectionIsMultiline
                    && !IsCurrentLineReadOnly
                    && (_textEditor.SelectionStartColumn - _promptLength - 1 >= 0)
                    && (_textEditor.SelectionEndColumn - _promptLength - 1 >= 0));
            }
        }

        /// <summary>
        /// The home position is at the start of the line after the prompt.
        /// </summary>
        void MoveToHomePosition()
        {
            _textEditor.Line = _textEditor.TotalLines;
            _textEditor.Column = _promptLength + 1;
        }

        /// <summary>
        /// Shows the previous command line in the command line history.
        /// </summary>
        void MoveToPreviousCommandLine()
        {
            if (_commandLineHistory.MovePrevious())
            {
                ReplaceCurrentLineTextAfterPrompt(_commandLineHistory.Current);
            }
        }

        /// <summary>
        /// Shows the next command line in the command line history.
        /// </summary>
        void MoveToNextCommandLine()
        {
            _textEditor.Line = _textEditor.TotalLines;
            if (_commandLineHistory.MoveNext())
            {
                ReplaceCurrentLineTextAfterPrompt(_commandLineHistory.Current);
            }
        }

        /// <summary>
        /// Replaces the current line text after the prompt with the specified text.
        /// </summary>
        void ReplaceCurrentLineTextAfterPrompt(string text)
        {
            string currentLine = GetCurrentLine();
            _textEditor.Replace(_promptLength, currentLine.Length, text);

            // Put cursor at end.
            _textEditor.Column = _promptLength + text.Length + 1;
        }
    }
}
