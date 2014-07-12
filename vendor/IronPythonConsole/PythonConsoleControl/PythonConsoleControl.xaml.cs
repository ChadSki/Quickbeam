using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Xml;


namespace PythonConsoleControl
{
    /// <summary>
    /// Interaction logic for PythonConsoleControl.xaml
    /// </summary>
    public partial class IronPythonConsoleControl : UserControl
    {
        readonly PythonConsolePad _pad;
        
        public IronPythonConsoleControl()
        {
            InitializeComponent();
            _pad = new PythonConsolePad();
            Grid.Children.Add(_pad.Control);
            // Load our custom highlighting definition
            IHighlightingDefinition pythonHighlighting;
            using (var s = typeof(IronPythonConsoleControl).Assembly.GetManifestResourceStream("PythonConsoleControl.Resources.Python.xshd"))
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
            HighlightingManager.Instance.RegisterHighlighting("Python Highlighting", new[] { ".cool" }, pythonHighlighting);
            _pad.Control.SyntaxHighlighting = pythonHighlighting;
            var transformers = _pad.Control.TextArea.TextView.LineTransformers;
            for (var i = 0; i < transformers.Count; ++i)
            {
                if (transformers[i] is HighlightingColorizer) transformers[i] = new PythonConsoleHighlightingColorizer(pythonHighlighting.MainRuleSet, _pad.Control.Document);
            }
        }

        /// <summary>
        /// Performs the specified action on the console host but only once the console
        /// has initialized.
        /// </summary>
        public void WithHost(Action<PythonConsoleHost> hostAction)
        {
            _hostAction = hostAction;
            Host.ConsoleCreated += Host_ConsoleCreated;
        }

        Action<PythonConsoleHost> _hostAction;

        void Host_ConsoleCreated(object sender, EventArgs e)
        {
            Console.ConsoleInitialized += Console_ConsoleInitialized;
        }

        void Console_ConsoleInitialized(object sender, EventArgs e)
        {
            _hostAction.Invoke(Host);
        }

        public PythonConsole Console
        {
            get { return _pad.Console; }
        }

        public PythonConsoleHost Host
        {
            get { return _pad.Host; }
        }

        public PythonConsolePad Pad
        {
            get { return _pad; }
        }
    }
}
