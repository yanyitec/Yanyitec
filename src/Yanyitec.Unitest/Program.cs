using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Unitest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(typeof(int?).GetHashCode());
            Console.WriteLine(typeof(DateTime?).GetHashCode());
        }
    }
}
