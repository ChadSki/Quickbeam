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
            var htag = hmap.getArbitraryTag();
            Console.WriteLine(htag);
            var tagHeader = htag.Header;
            Console.WriteLine(tagHeader);
            var firstClass = tagHeader.Get("first_class");
            Console.WriteLine(firstClass);
            var tagData = htag.Data;
            Console.WriteLine(tagData);
            var marker_field = tagData.Fields[0] as StringField;
            Console.WriteLine(marker_field.Value);
            marker_field.Value = "cheese";
            Console.WriteLine(marker_field.Value);
        }
    }
}
