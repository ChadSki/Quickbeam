using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace IronPythonConsole
{
    /// <summary>
    /// Interaction logic for ReplGrid.xaml
    /// </summary>
    public partial class ReplGrid : UserControl
    {
        ConsoleOptions consoleOptionsProvider;
        
        public ReplGrid()
		{
            Initialized += new EventHandler(MainWindow_Initialized);
            // Load our custom highlighting definition:
            IHighlightingDefinition pythonHighlighting;
            using (Stream s = typeof(ReplGrid).Assembly.GetManifestResourceStream("IronPythonConsole.Resources.Python.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    pythonHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Python Highlighting", new string[] { ".cool" }, pythonHighlighting);
            	
			InitializeComponent();

            textEditor.SyntaxHighlighting = pythonHighlighting;

            textEditor.PreviewKeyDown += new KeyEventHandler(textEditor_PreviewKeyDown);

            consoleOptionsProvider = new ConsoleOptions(console.Pad);

            propertyGridComboBox.SelectedIndex = 0;

            expander.Expanded += new RoutedEventHandler(expander_Expanded);

            console.Pad.Host.ConsoleCreated +=new PythonConsoleControl.ConsoleCreatedEventHandler(Host_ConsoleCreated);
		}

		string currentFileName;

        void Host_ConsoleCreated(object sender, EventArgs e)
        {
            console.Pad.Console.ConsoleInitialized += new PythonConsoleControl.ConsoleInitializedEventHandler(Console_ConsoleInitialized);
        }

        void Console_ConsoleInitialized(object sender, EventArgs e)
        {
            string startupScipt = "import IronPythonConsole";
            ScriptSource scriptSource = console.Pad.Console.ScriptScope.Engine.CreateScriptSourceFromString(startupScipt, SourceCodeKind.Statements);
            try
            {
                scriptSource.Execute();
            }
            catch {}
            //double[] test = new double[] { 1.2, 4.6 };
            //console.Pad.Console.ScriptScope.SetVariable("test", test);
        }

        void MainWindow_Initialized(object sender, EventArgs e)
        {
            //propertyGridComboBox.SelectedIndex = 1;
        }
		
		void openFileClick(object sender, RoutedEventArgs e)
		{   
            OpenFileDialog dlg = new OpenFileDialog();
			dlg.CheckFileExists = true;
			if (dlg.ShowDialog() ?? false) {
				currentFileName = dlg.FileName;
				textEditor.Load(currentFileName);
				//textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
			}
		}
		
		void saveFileClick(object sender, EventArgs e)
		{
			if (currentFileName == null) {
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.DefaultExt = ".txt";
				if (dlg.ShowDialog() ?? false) {
					currentFileName = dlg.FileName;
				} else {
					return;
				}
			}
			textEditor.Save(currentFileName);
		}

        void runClick(object sender, EventArgs e)
        {
            RunStatements();
        }

        void textEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5) RunStatements();
        }

        void RunStatements()
        {
            string statementsToRun = "";
            if (textEditor.TextArea.Selection.Length > 0)
                statementsToRun = textEditor.TextArea.Selection.GetText(textEditor.TextArea.Document);
            else
                statementsToRun = textEditor.TextArea.Document.Text;
            console.Pad.Console.RunStatements(statementsToRun);
        }
		
		void propertyGridComboBoxSelectionChanged(object sender, RoutedEventArgs e)
		{
            if (propertyGrid == null)
				return;
			switch (propertyGridComboBox.SelectedIndex) {
				case 0:
                    propertyGrid.SelectedObject = consoleOptionsProvider; // not .Instance
					break;
				case 1:
					//propertyGrid.SelectedObject = textEditor.Options; (for WPF native control)
                    propertyGrid.SelectedObject = textEditor.Options;
					break;
			}
		}

        void expander_Expanded(object sender, RoutedEventArgs e)
        {
            propertyGridComboBoxSelectionChanged(sender, e);
        }
		
    }
}
