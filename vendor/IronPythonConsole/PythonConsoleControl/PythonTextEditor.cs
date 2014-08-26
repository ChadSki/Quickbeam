// Copyright (c) 2010 Joe Moorhouse

using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows.Threading;
using System.Windows.Input;
using System.Threading;
using System.Diagnostics;

namespace Quickbeam.PythonConsoleControl
{
    /// <summary>
    /// Interface console to AvalonEdit and handle autocompletion.
    /// </summary>
    public class PythonTextEditor
    {
        internal TextEditor TextEditor;
        internal TextArea TextArea;
        readonly StringBuilder _writeBuffer = new StringBuilder();
        volatile bool _writeInProgress;
        PythonConsoleCompletionWindow _completionWindow;
        private const int CompletionEventIndex = 0;
        private const int DescriptionEventIndex = 1;
        readonly WaitHandle[] _completionWaitHandles;
        readonly AutoResetEvent _completionRequestedEvent = new AutoResetEvent(false);
        readonly AutoResetEvent _descriptionRequestedEvent = new AutoResetEvent(false);
        Thread completionThread;
        PythonConsoleCompletionDataProvider _completionProvider;

        public PythonTextEditor(TextEditor textEditor)
        {
            TextEditor = textEditor;
            TextArea = textEditor.TextArea;
            _completionWaitHandles = new WaitHandle[] { _completionRequestedEvent, _descriptionRequestedEvent };
            completionThread = new Thread(Completion)
            {
                Priority = ThreadPriority.Lowest,
                IsBackground = true
            };
            completionThread.Start();
        }

        public bool WriteInProgress
        {
            get { return _writeInProgress; }
        }

        public ICollection<CommandBinding> CommandBindings
        {
            get
            {
                var textAreaDefaultInputHandler = TextArea.ActiveInputHandler as TextAreaDefaultInputHandler;
                return textAreaDefaultInputHandler == null ? null : textAreaDefaultInputHandler.CommandBindings;
            }
        }

        public void Write(string text)
        {
            Write(text, false);
        }

        public void Write(string text, bool allowSynchronous)
        {
            //text = text.Replace("\r\r\n", "\r\n");
            text = text.Replace("\r\r\n", "\r");
            text = text.Replace("\r\n", "\r");
            if (allowSynchronous)
            {
                MoveToEnd();
                PerformTextInput(text);
                return;
            }
            lock (_writeBuffer)
            {
                _writeBuffer.Append(text);
            }
            if (_writeInProgress) return;
            _writeInProgress = true;
            ThreadPool.QueueUserWorkItem(CheckAndOutputWriteBuffer);
            Stopwatch.StartNew();
        }

        private void CheckAndOutputWriteBuffer(Object stateInfo)
        {
            var writeCompletedEvent = new AutoResetEvent(false);
            Action action = delegate()
            {
                string toWrite;
                lock (_writeBuffer)
                {
                    toWrite = _writeBuffer.ToString();
                    _writeBuffer.Remove(0, _writeBuffer.Length);
                    //writeBuffer.Clear();
                }
                MoveToEnd();
                PerformTextInput(toWrite);
                writeCompletedEvent.Set();
            };
            while (true)
            {
                // Clear writeBuffer and write out.
                TextArea.Dispatcher.BeginInvoke(action, DispatcherPriority.Normal);
                // Check if writeBuffer has refilled in the meantime; if so clear and write out again.
                writeCompletedEvent.WaitOne();
                lock (_writeBuffer)
                {
                    if (_writeBuffer.Length != 0) continue;
                    _writeInProgress = false;
                    break;
                }
            }
        }

        private void MoveToEnd()
        {
            var lineCount = TextArea.Document.LineCount;
            if (TextArea.Caret.Line != lineCount) TextArea.Caret.Line = TextArea.Document.LineCount;
            var column = TextArea.Document.Lines[lineCount - 1].Length + 1;
            TextArea.Caret.Column = column;
        }

        private void PerformTextInput(string text)
        {
            if (text == "\n" || text == "\r\n")
            {
                var newLine = TextUtilities.GetNewLineFromDocument(TextArea.Document, TextArea.Caret.Line);
                TextEditor.AppendText(newLine);
            }
            else
                TextEditor.AppendText(text);
            TextArea.Caret.BringCaretToView();
        }

        public int Column
        {
            get { return TextArea.Caret.Column; }
            set { TextArea.Caret.Column = value; }
        }

        /// <summary>
        /// Gets the current cursor line.
        /// </summary>
        public int Line
        {
            get { return TextArea.Caret.Line; }
            set { TextArea.Caret.Line = value; }
        }

        /// <summary>
        /// Gets the total number of lines in the text editor.
        /// </summary>
        public int TotalLines
        {
            get { return TextArea.Document.LineCount; }
        }

        public delegate string StringAction();
        /// <summary>
        /// Gets the text for the specified line.
        /// </summary>
        public string GetLine(int index)
        {
            return (string)TextArea.Dispatcher.Invoke(new StringAction(delegate()
            {
                var line = TextArea.Document.Lines[index];
                return TextArea.Document.GetText(line);
            }));
        }

        /// <summary>
        /// Replaces the text at the specified index on the current line with the specified text.
        /// </summary>
        public void Replace(int index, int length, string text)
        {
            //int currentLine = textArea.Caret.Line - 1;
            var currentLine = TextArea.Document.LineCount - 1;
            var startOffset = TextArea.Document.Lines[currentLine].Offset;
            TextArea.Document.Replace(startOffset + index, length, text); 
        }

        public event TextCompositionEventHandler TextEntering
        {
            add { TextArea.TextEntering += value; }
            remove { TextArea.TextEntering -= value; }
        }

        public event TextCompositionEventHandler TextEntered
        {
            add { TextArea.TextEntered += value; }
            remove { TextArea.TextEntered -= value; }
        }

        public event KeyEventHandler PreviewKeyDown
        {
            add { TextArea.PreviewKeyDown += value; }
            remove { TextArea.PreviewKeyDown -= value; }
        }

        public int SelectionStart
        {
            get
            {
                return TextArea.Selection.SurroundingSegment.Offset;
            }
        }

        public int SelectionLength
        {
            get
            {
                return TextArea.Selection.Length;
            }
        }

        public bool SelectionIsMultiline
        {
            get
            {
                return TextArea.Selection.IsMultiline(TextArea.Document);
            }
        }

        public int SelectionStartColumn
        {
            get
            {
                int startOffset = TextArea.Selection.SurroundingSegment.Offset;
                return startOffset - TextArea.Document.GetLineByOffset(startOffset).Offset + 1;
            }
        }

        public int SelectionEndColumn
        {
            get
            {
                int endOffset = TextArea.Selection.SurroundingSegment.EndOffset;
                return endOffset - TextArea.Document.GetLineByOffset(endOffset).Offset + 1;
            }
        }

        public PythonConsoleCompletionDataProvider CompletionProvider
        {
            get { return _completionProvider; }
            set { _completionProvider = value; }
        }

        public Thread CompletionThread
        {
            get { return completionThread; }
        }

        public bool StopCompletion()
        {
            if (_completionProvider.AutocompletionInProgress)
            {
                // send Ctrl-C abort
                completionThread.Abort(new Microsoft.Scripting.KeyboardInterruptException(""));
                return true;
            }
            return false;
        }

        public PythonConsoleCompletionWindow CompletionWindow
        {
            get { return _completionWindow; }
        }

        public void ShowCompletionWindow()
        {
            _completionRequestedEvent.Set();
        }

        public void UpdateCompletionDescription()
        {
            _descriptionRequestedEvent.Set();
        }

        /// <summary>
        /// Perform completion actions on the background completion thread.
        /// </summary>
        void Completion()
        {
            while (true)
            {
                var action = WaitHandle.WaitAny(_completionWaitHandles);
                if (action == CompletionEventIndex && _completionProvider != null) BackgroundShowCompletionWindow();
                if (action == DescriptionEventIndex && _completionProvider != null && _completionWindow != null) BackgroundUpdateCompletionDescription();
            }
        }

        /// <summary>
        /// Obtain completions (this runs in its own thread)
        /// </summary>
        internal void BackgroundShowCompletionWindow() //ICompletionItemProvider
        {
			// provide AvalonEdit with the data:
            var itemForCompletion = "";
            TextArea.Dispatcher.Invoke(new Action(delegate()
            {
                var line = TextArea.Document.Lines[TextArea.Caret.Line - 1];
                itemForCompletion = TextArea.Document.GetText(line);
            }));

            var completions = _completionProvider.GenerateCompletionData(itemForCompletion);

            if (completions != null && completions.Length > 0) TextArea.Dispatcher.BeginInvoke(new Action(delegate()
            {
                _completionWindow = new PythonConsoleCompletionWindow(TextArea, this);
                var data = _completionWindow.CompletionList.CompletionData;
                foreach (var completion in completions)
                {
                    data.Add(completion);
                }
                _completionWindow.Show();
                _completionWindow.Closed += delegate
                {
                    _completionWindow = null;
                };
            }));
            
        }

        internal void BackgroundUpdateCompletionDescription()
        {
            _completionWindow.UpdateCurrentItemDescription();
        }

        public void RequestCompletioninsertion(TextCompositionEventArgs e)
        {
            if (_completionWindow != null) _completionWindow.CompletionList.RequestInsertion(e);
            // if autocompletion still in progress, terminate
            StopCompletion();
        }

    }
}

    
   
