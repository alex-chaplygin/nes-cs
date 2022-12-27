using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES.Mappers
{
    class MMC5
    {
        delegate void SwitchOp(byte val);
        enum PRGMODE
        {
            One32,
            Two16,
            One16Two8,
            Four8
        }
        enum CHRMODE
        {
            chr8,
            chr4,
            chr2,
            chr1
        }
        static CHRMODE chr_mode;
        static PRGMODE prg_mode;
        struct SwitchReg
        {
            
            public int value;
            public SwitchOp write;
            public SwitchReg( int v, SwitchOp r)
            {
                value = v;
                write = r;
            }
        }

        static SwitchReg[] switchTable = new SwitchReg[]
        {
	    new SwitchReg( 0x5100, PRGmode),
	    new SwitchReg( 0x5101, CHRmode),
	    new SwitchReg( 0x5102, PRGramp1),
	    new SwitchReg( 0x5103, PRGramp2),
        };
        public static void Write(ushort adr, byte val)
        {
            for (int i = 0; i < switchTable.Length; i++)
            {
                if (adr == switchTable[i].value)
                {
                    switchTable[i].write(val);
                    return;
                }
            }
        }
        static void PRGmode(byte val)
        {
            prg_mode = (PRGMODE)(val & 0x3);     
        }
        static void CHRmode(byte val)
        {
            chr_mode = (CHRMODE)(val & 0x3);
        }
        static void PRGramp1(byte val)
        {

        }
        static void PRGramp2(byte val)
        {

        }
    }
}
