using System;
using System.Linq;
using NimbusSharp;
using System.Collections.Generic;

namespace ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Workbench.Instance.OpenMap();
            var hmap = Workbench.Instance.Maps[0];
            Console.WriteLine(hmap);
            var tags = Workbench.Instance.Maps[0].Tags();
            var htag = tags.First();
            Console.WriteLine(htag);
            var tagHeader = htag.Header;
            Console.WriteLine(tagHeader);
            //var firstClass = tagHeader.Get("first_class");
            //Console.WriteLine(firstClass);
            //var tagData = htag.Data;
            //Console.WriteLine(tagData);
            //var marker_field = tagData.Fields[0] as StringField;
            //Console.WriteLine(marker_field.Value);
            //marker_field.Value = "cheese";
            //Console.WriteLine(marker_field.Value);

            Console.ReadKey();
        }
    }
}
