// Copyright (c) 2010 Joe Moorhouse

using ICSharpCode.AvalonEdit;
using System.Windows.Media;

namespace PythonConsoleControl
{   
    public class PythonConsolePad 
    {
        public PythonConsolePad()
        {
            var brushConverter = new BrushConverter();

            Control = new TextEditor
            {
                Foreground = (SolidColorBrush)brushConverter.ConvertFromString("#c5c8c6"),
                Background = (SolidColorBrush)brushConverter.ConvertFromString("#212121"),
                FontSize = 9
            };
            var pythonTextEditor = new PythonTextEditor(Control);
            Host = new PythonConsoleHost(pythonTextEditor);
            Host.Run();
            Control.FontFamily = new FontFamily("Consolas");
            Control.FontSize = 12;
        }

        public TextEditor Control { get; private set; }

        public PythonConsoleHost Host { get; private set; }

        public PythonConsole Console
        {
            get { return Host.Console; }
        }

        public void Dispose()
        {
            Host.Dispose();
        }
    }
}
