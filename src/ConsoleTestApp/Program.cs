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
            foreach (var x in tagHeader.FieldTypes)
            {
                Console.WriteLine(x);
            }
            var firstClass = tagHeader["first_class"];
            Console.WriteLine(firstClass);
            var tagData = htag.TagData;
            Console.WriteLine(tagData);
            //var marker_field = tagData.Fields[0] as StringField;
            //Console.WriteLine(marker_field.Value);
            //marker_field.Value = "cheese";
            //Console.WriteLine(marker_field.Value);

            Console.ReadKey();
        }
    }
}
