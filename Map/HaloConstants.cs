using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quickbeam
{
    static class HaloConstants
    {
        public static List<string> Classes
        {
            get
            {
                var asdf = new List<string>();
                asdf.Add("weap");
                asdf.Add("vehi");
                return asdf;
            }
        }
    }
}
