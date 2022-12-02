﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES
{
    public enum Banks
    {
        Chr0,
        Chr1,
        Prg
    }
    public class MMC1
    {
        delegate void SwitchOp();
        struct SwitchReg
        {
            public int up;
            public int down;
            public SwitchOp bank;
            public SwitchReg(int p ,int u, SwitchOp r)
            {
                up = p;
                down = u;
                bank = r;
            }
        }

       

        /// <summary>
        /// Номер банка для переключения 
        /// </summary>
        public static byte registr = 0x10;
        public static int bank;
        public static Banks sw;
        public static void Write(ushort adr, byte val)
        {
            Console.WriteLine($"val = {val:x}");
            if ((val & 0x80) > 0)
                registr = 0x10;
             else if ((registr & 1) > 0)
            {
                registr >>= 1;
                val &= 1;
                val <<= 4;
                registr |= val;
                Console.WriteLine($"switch val = {val:x} reg = {registr:X}");
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
            sw = Banks.Chr0;
	    PPU.WritePattern0(Cartridge.GetChrBank(bank));
        }

        public static void Chr1()
        {
            sw = Banks.Chr1;
	    PPU.WritePattern1(Cartridge.GetChrBank(bank));
        }

	public static void Prg()
        {
            sw = Banks.Prg;
	    Memory.WriteROM1(Cartridge.GetPrgBank(bank));
        }

	public static void Switch(ushort adr)
        {
            bank = registr; 
            SwitchReg[] switchTable = new SwitchReg[]
            {
                    new SwitchReg( 0xA000, 0xBFFF, Chr0 ),
                    new SwitchReg (0xC000, 0xDFFF, Chr1 ),
                    new SwitchReg(0xE000, 0xFFFF, Prg )
            };

            for (int i = 0; i < switchTable.Length; i++)
                if (switchTable[i].up <= adr && adr  <= switchTable[i].down)
                    switchTable[i].bank();
        }
    }
}
