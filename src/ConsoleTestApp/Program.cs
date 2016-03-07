using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonBinding;

namespace ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var foo = new HaloMapProxy();
            var bar = foo.getGhost();
            var qux = bar.getData();
        }
    }
}
