using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES
{
    class MMC1
	{
        public static byte registr = 0x10;
        public static void Write(ushort adr, byte val)
        {
            registr >>= 1;
            val &= 1;
        }
    }
}
