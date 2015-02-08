using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Scripting;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Quickbeam.Low.ByteArray;

namespace Quickbeam.IronPythonConsole
{
    /// <summary>
    /// Interaction logic for ReplGrid.xaml
    /// </summary>
    public partial class ReplGrid : UserControl
    {
        readonly ConsoleOptions _consoleOptionsProvider;
        
        public ReplGrid()
		{
            // Load our custom highlighting definition:
            IHighlightingDefinition pythonHighlighting;
            using (var s = typeof(ReplGrid).Assembly.GetManifestResourceStream("Quickbeam.IronPythonConsole.Resources.Python.xshd"))
            {
                if (s == null) throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                    pythonHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Python Highlighting", new[] { ".py" }, pythonHighlighting);
            	
			InitializeComponent();
            textEditor.SyntaxHighlighting = pythonHighlighting;
            textEditor.PreviewKeyDown += textEditor_PreviewKeyDown;
            _consoleOptionsProvider = new ConsoleOptions(Console.Pad);
            Console.Pad.Host.ConsoleCreated += Host_ConsoleCreated;
		}

		private string _currentFileName;

        void Host_ConsoleCreated(object sender, EventArgs e)
        {
            Console.Pad.Console.ConsoleInitialized += Console_ConsoleInitialized;
        }

        void Console_ConsoleInitialized(object sender, EventArgs e)
        {
            //Console.Pad.Console.ScriptScope.Engine.CreateScriptSourceFromString(
            //    "import IronPythonConsole\n" +
            //    "import halolib", SourceCodeKind.Statements)
            //    .Execute();
        }
		
		void OpenFileClick(object sender, RoutedEventArgs e)
		{   
            var dlg = new OpenFileDialog {CheckFileExists = true};
		    if (!((bool) dlg.ShowDialog())) return;
		    _currentFileName = dlg.FileName;
		    textEditor.Load(_currentFileName);
		    //textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
		}
		
		void SaveFileClick(object sender, EventArgs e)
		{
			if (_currentFileName == null) {
				var dlg = new SaveFileDialog {DefaultExt = ".txt"};
			    if (!((bool) dlg.ShowDialog())) return;
				_currentFileName = dlg.FileName;
			}
			textEditor.Save(_currentFileName);
		}

        void RunClick(object sender, EventArgs e)
        {
            RunStatements();
        }

        void textEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5) RunStatements();
        }

        void RunStatements()
        {
            var statementsToRun = textEditor.TextArea.Selection.Length > 0
                ? textEditor.TextArea.Selection.GetText(textEditor.TextArea.Document)
                : textEditor.TextArea.Document.Text;
            Console.Pad.Console.RunStatements(statementsToRun);
        }
    }
}
