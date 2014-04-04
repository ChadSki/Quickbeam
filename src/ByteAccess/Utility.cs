using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickbeam.ByteAccess
{
    internal static class Utility
    {
        /// <summary>
        /// Whether the provided byte array contains null
        /// </summary>
        /// <param name="array">byte array to search</param>
        /// <returns>true if found at least one null</returns>
        public static bool ContainsNull(this byte[] array)
        {
            foreach (byte b in array)
            {
                if (b == (byte)0x00)
                    return true;
            }
            return false;
        }
    }
}
