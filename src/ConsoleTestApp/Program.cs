using System;
using PythonBinding;

namespace ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(PythonInterpreter.Instance);
            PythonInterpreter.Instance.OpenMap(HaloMemory.PC);
            Console.WriteLine(PythonInterpreter.Instance);
            var hmap = PythonInterpreter.Instance.Children[0] as HaloMapProxy;
            Console.WriteLine(hmap);
            var htag = hmap.getGhost();
            Console.WriteLine(htag);
            var tagheader = htag.Header;
            Console.WriteLine(tagheader);
            var firstClass = tagheader.Get("first_class") as String;
            Console.WriteLine(firstClass);
        }
    }
}
