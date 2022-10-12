using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES
{
    public static class CPU
    {
        static byte A;
        static byte X;
        static byte Y;
        static byte ST;
	static ushort PC;
	static byte SP;

        static byte Fetch()
        {
            return 0;
        }
    }
}
