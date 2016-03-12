using PythonBinding;

namespace ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            PythonInterpreter.Instance.OpenMap(HaloMemory.PC);
            var map = PythonInterpreter.Instance.Maps[0];
            var bar = map.getGhost();
            var qux = bar.getData();
        }
    }
}
