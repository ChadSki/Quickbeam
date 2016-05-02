using ICSharpCode.TreeView;
using PythonBinding;

namespace HalolibWrapper
{
    public enum HaloMemory { PC, CE }

    public class PythonInterpreter : SharpTreeNode
    {
        public static PythonInterpreter Instance { get; private set; } = new PythonInterpreter();

        private PyObj HalolibModule { get; set; }

        private PythonInterpreter()
        {
            var mainModule = PythonBinding.PythonInterpreter.Initialize();
            HalolibModule = mainModule.GetAttrString("halolib");
        }

        public void OpenMap(HaloMemory whichExe)
        {
            System.Console.WriteLine("asdf");
            var methodName = whichExe == HaloMemory.PC ? "from_hpc" : "from_hce";
            var halomapClass = HalolibModule.Module_GetAttr("HaloMap");
            var map = halomapClass.CallMethod(methodName, null);
            Children.Add(new HaloMapNode(map));
        }
    }
}
