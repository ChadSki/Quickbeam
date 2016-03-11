using PythonBinding;

namespace ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var map = PythonInterpreter.Instance.OpenMap(HaloMemory.PC);
            var bar = map.getGhost();
            var qux = bar.getData();
        }
    }
}
