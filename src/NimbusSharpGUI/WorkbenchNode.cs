using ICSharpCode.TreeView;
using NimbusSharp;

namespace NimbusSharpGUI
{
    public enum HaloMemory { PC, CE }

    public class WorkbenchNode : SharpTreeNode
    {
        public static WorkbenchNode Instance { get; private set; } = new WorkbenchNode();

        public override object Text { get { return "Workbench"; } }

        private WorkbenchNode()
        {
            LazyLoading = true;
        }

        protected override void LoadChildren()
        {
            OpenMap();
        }

        public void OpenMap()
        {
            var map = Workbench.Instance.OpenMap();
            var mapNode = new HaloMapNode(map);
            Children.Add(mapNode);
        }
    }
}
