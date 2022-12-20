using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES.Mappers
{
    public class MMC2
    {
        delegate void SwitchOp(byte val);

        struct SwitchReg
        {
            public int up;
            public int down;
            public SwitchOp write;
            public SwitchReg(int p, int u, SwitchOp r)
            {
                up = p;
                down = u;
                write = r;
            }
        }

        static SwitchReg[] switchTable = new SwitchReg[]
        {
	    /*new SwitchReg( 0xA000, 0xAFFF, PRG),
                new SwitchReg (0xB000, 0xBFFF, CHR1),
                new SwitchReg(0xC000, 0xCFFF, CHR2),
                new SwitchReg(0xD000, 0xDFFF, CHR3),
                new SwitchReg(0xE000, 0xEFFF, CHR4),
                new SwitchReg(0xF000, 0xFFFF, Mirr)*/
        };

        public static void Write(ushort adr, byte val)
        {
            for (int i = 0; i < switchTable.Length; i++)
            {
                if (adr < switchTable[i].up)
                {
                    switchTable[i].write(val);
                    return;
                }
            }
        }

        static void CHR1()
        {

        }

        static void CHR2()
        {

        }

        static void CHR3()
        {

        }

        static void CHR4()
        {

        }

        static void Mirr()
        {

            
        }

        static void PRG()
        {

        }
    }
}
