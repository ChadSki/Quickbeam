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
            var hmap = PythonInterpreter.Instance.Children[0];
            Console.WriteLine(hmap);
            //var htag = hmap.getGhost();
            //Console.WriteLine(htag);
            //var hstruct = htag.Data;
            //Console.WriteLine(hstruct);
        }
    }
}
