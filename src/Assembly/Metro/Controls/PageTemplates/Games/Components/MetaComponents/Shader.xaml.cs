using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using Assembly.Metro.Dialogs;
using Blamite.Blam.Shaders;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
    /// <summary>
    ///     Interaction logic for Shader.xaml
    /// </summary>
    public partial class Shader : UserControl
    {
        public Shader()
        {
            InitializeComponent();
        }

        private void btnDisassemble_Click(object sender, RoutedEventArgs e)
        {
            var shaderRef = (ShaderRef) DataContext;
            IShader shader = shaderRef.Shader;
            if (shader == null)
                return;

            string xsdPath = App.AssemblyStorage.AssemblySettings.XsdPath;
            if (string.IsNullOrWhiteSpace(xsdPath) || !File.Exists(xsdPath))
            {
                MetroMessageBox.Show(
                    "xsd.exe (from the XDK) is required in order to disassemble shaders.\r\nYou can set a path to it in Settings under Map Editor.");
                return;
            }

            string microcodePath = Path.GetTempFileName();
            try
            {
                // Write the microcode to a file so XSD can use it
                File.WriteAllBytes(microcodePath, shader.Microcode);

                // Start XSD.exe with one of the /raw switches (depending upon shader type)
                // and the microcode file
                var startInfo = new ProcessStartInfo(xsdPath)
                {
                    Arguments = shaderRef.Type == ShaderType.Pixel ? "/rawps" : "/rawvs"
                };

                // Add the path to the microcode file
                startInfo.Arguments += " \"" + microcodePath + "\"";
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;

                // Run it and capture the output
                Process process = Process.Start(startInfo);
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Display it
                MetroMessageBoxCode.Show("Shader Disassembly", output);
            }
            finally
            {
                File.Delete(microcodePath);
            }
        }
    }
}