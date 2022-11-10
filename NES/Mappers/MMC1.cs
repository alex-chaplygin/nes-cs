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
            if (val == (val & 0x80))
                registr = 0x10;
            else if ((registr & 1) > 0)
            {
                registr >>= 1;
                val &= 1;
                val <<= 4;
                registr |= val;
                // переключение
                registr = 0x10;
            }
            else
            {
                registr >>= 1;
                val &= 1;
                val <<= 4;
                registr |= val;
            }
        }
    }
}
