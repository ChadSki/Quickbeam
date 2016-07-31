using ICSharpCode.TreeView;
using CrappyCppBinding;
using System.Collections.Specialized;

namespace HalolibWrapper
{
    public enum HaloMemory { PC, CE }

    public class PythonInterpreter : SharpTreeNode
    {
        public static PythonInterpreter Instance { get; private set; } = new PythonInterpreter();

        public override object Text { get { return "Python Interpreter"; } }

        private PyObj HalolibModule { get; set; }

        private PythonInterpreter()
        {
            LazyLoading = true;
            var mainModule = CrappyCppBinding.PythonInterpreter.Initialize();
            HalolibModule = mainModule.GetAttrString("halolib");
        }

        protected override void LoadChildren()
        {
            var halomapClass = HalolibModule.Module_GetAttr("HaloMap");
            var map = halomapClass.CallMethod("from_hpc", null);
            Children.Add(new HaloMapNode(map));
        }

        public void OpenMap(HaloMemory whichExe)
        {
            var methodName = whichExe == HaloMemory.PC ? "from_hpc" : "from_hce";
            var halomapClass = HalolibModule.Module_GetAttr("HaloMap");
            var map = halomapClass.CallMethod(methodName, null);
            Children.Add(new HaloMapNode(map));
            Children.Add(new HaloMapNode(map));
        }
    }
}
