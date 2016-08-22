using NimbusSharp;

namespace NimbusSharpGUI.MapExplorer
{
    public class WorkbenchNode : ExplorerNode
    {
        public static WorkbenchNode Instance { get; private set; } = new WorkbenchNode();

        private WorkbenchNode()
        {
            foreach (var map in Workbench.Instance.Maps)
            {
                Children.Add(new HaloMapNode(map, map.ToString()));
            }
        }

        public override string Name { get { return "Workbench"; } }

        public void OpenMap()
        {
            var map = Workbench.Instance.OpenMap();
            Children.Add(new HaloMapNode(map, "Halo PC Memory"));
        }
    }
}
