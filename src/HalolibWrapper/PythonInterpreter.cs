﻿using ICSharpCode.TreeView;
using PythonBinding;

namespace HalolibWrapper
{
    class PythonInterpreter : SharpTreeNode
    {
        public static PythonInterpreter Instance { get; private set; } = new PythonInterpreter();

        private PyObj HalolibModule { get; set; }

        private PythonInterpreter()
        {
            var mainModule = PythonBinding.PythonInterpreter.Initialize();
            HalolibModule = mainModule.GetAttrString("halolib");
        }

        public enum HaloMemory { PC, CE }

        public void OpenMap(HaloMemory whichExe)
        {
            var methodName = whichExe == HaloMemory.PC ? "from_hpc" : "from_hce";
            var map = HalolibModule.GetAttrString("HaloMap").CallMethod(methodName, null);
            Children.Add(new HaloMapNode(map));
        }
    }
}
