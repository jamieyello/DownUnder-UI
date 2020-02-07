using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.Utilities
{
    internal static class ConsoleOutput
    {
        internal static void WriteLine(string warning)
        {
            Console.WriteLine("DownUnder WARNING: " + warning);
        }
    }
}
