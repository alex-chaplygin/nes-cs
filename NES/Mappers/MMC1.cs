using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES
{
    public class MMC1
    {
        delegate void SwitchOp();
        struct SwitchReg
        {
            public int down;
            public SwitchOp bank;
            public SwitchReg(int u, SwitchOp r)
            {
                down = u;
                bank = r;
            }
        }
       
        /// <summary>
        /// Номер банка для переключения 
        /// </summary>
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
                Console.WriteLine($"val = {val:x} reg = {registr:X}");
                // переключение
                Switch(adr);
                registr = 0x10;
            }
            else
            {
                registr >>= 1;
                val &= 1;
                val <<= 4;
                registr |= val;
                Console.WriteLine($"val = {val:x} reg = {registr:X}");
             
            }
        }

        public static void Chr0()
        {

        }
        public static void Chr1()
        {

        }
        public static void Prg()
        {

        }
        public static void Switch(ushort adr)
        {
            SwitchReg[] switchTable = new SwitchReg[]
            {
                    new SwitchReg( 0xbfff, Chr0 ),
                    new SwitchReg (0xffff, Chr1 ),
                    new SwitchReg( 0xffff, Prg )
            };

            for (int i = 0; i < switchTable.Length; i++)
                if (adr < switchTable[i].down)
                    switchTable[i].bank();
        }
    }
}
