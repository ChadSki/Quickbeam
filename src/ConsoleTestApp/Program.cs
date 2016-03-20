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
            var hmap = PythonInterpreter.Instance.Children[0] as HaloMapNode;
            Console.WriteLine(hmap);
            var htag = hmap.getGhost();
            Console.WriteLine(htag);
            var tagHeader = htag.Header;
            Console.WriteLine(tagHeader);
            var firstClass = tagHeader.Get("first_class");
            Console.WriteLine(firstClass);
            var tagData = htag.Data;
            Console.WriteLine(tagData);
        }
    }
}
