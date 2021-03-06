﻿using System;
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
            //var tags = Workbench.Instance.Maps[0].Tags();
            //var htag = tags.First();
            var htag = hmap.ArbitraryTag;
            Console.WriteLine(htag);
            var tagHeader = htag.Header;
            Console.WriteLine(tagHeader);
            var firstClass = tagHeader["first_class"];
            Console.WriteLine(firstClass);
            var tagData = htag.TagData;
            Console.WriteLine(tagData);
            Console.ReadKey();
        }
    }
}
