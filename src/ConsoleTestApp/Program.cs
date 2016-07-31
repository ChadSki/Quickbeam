using System;
using NimbusSharp;

namespace ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(HalolibWrapper.PythonInterpreter.Instance);
            //HalolibWrapper.PythonInterpreter.Instance.OpenMap(HalolibWrapper.HaloMemory.PC);
            //Console.WriteLine(HalolibWrapper.PythonInterpreter.Instance);
            //var hmap = HalolibWrapper.PythonInterpreter.Instance.Children[0] as HalolibWrapper.HaloMapNode;
            //Console.WriteLine(hmap);
            //var htag = hmap.GetArbitraryTag();
            //Console.WriteLine(htag);
            //var tagHeader = htag.Header;
            //Console.WriteLine(tagHeader);
            //var firstClass = tagHeader.Get("first_class");
            //Console.WriteLine(firstClass);
            //var tagData = htag.Data;
            //Console.WriteLine(tagData);
            //var marker_field = tagData.Fields[0] as StringField;
            //Console.WriteLine(marker_field.Value);
            //marker_field.Value = "cheese";
            //Console.WriteLine(marker_field.Value);

            var x = PythonInterpreter.MainModule;
            Console.WriteLine(x.ToString());
            Console.ReadKey();
        }
    }
}
