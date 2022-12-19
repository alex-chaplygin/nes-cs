﻿﻿using System;
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
        enum PRGMode
        {
            Switch32,
            FixFirst,
            FixLast
        }
        enum CHRmode 
        {
            Switch8,
            SwitchTwo4Banks
        }

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
        public static byte control;
        public static int bank;
        public static Banks sw;
        static PRGMode prg_mode;
        static CHRmode chr_mode;
        public static void Write(ushort adr, byte val)
        {
            if (0x8000 <= adr && adr <= 0x9FFF)
                Control(val);

            Console.WriteLine($"val = {val:x}");
            if ((val & 0x80) > 0)
            {
                registr = 0x10;
                Control((byte) (control | 0x0C));
            }
                
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
            switch (chr_mode) 
            { 
                case CHRmode.Switch8: PPU.WritePattern0(Cartridge.GetChrBank(bank & 0xE));break;
                case CHRmode.SwitchTwo4Banks:PPU.WritePattern0(Cartridge.GetChrBank4bytes(bank)); break;
            }
        }

        public static void Chr1()
        {
            sw = Banks.Chr1;
	        PPU.WritePattern1(Cartridge.GetChrBank(bank));
            switch (chr_mode)
            {
                case CHRmode.SwitchTwo4Banks:PPU.WritePattern0(Cartridge.GetChrBank4bytes(bank)); break;
            }
        }

        public static void Prg()
        {
            sw = Banks.Prg;
            switch (prg_mode) 
            {
                case PRGMode.Switch32:
                    Memory.WriteROM1(Cartridge.GetPrgBank(bank & 0xE));
                    Memory.WriteROM2(Cartridge.GetPrgBank((bank & 0xE) + 1));break;
                case PRGMode.FixFirst: Memory.WriteROM1(Cartridge.GetPrgBank(0));
                    Memory.WriteROM2(Cartridge.GetPrgBank(bank)); break;
                case PRGMode.FixLast: Memory.WriteROM1(Cartridge.GetPrgBank(bank));
                    Memory.WriteROM2(Cartridge.GetPrgBank(Cartridge.prg_count - 1)); break;
            }

        }

        public static void Control(byte val)
        {
            switch (val & 3) 
            {
                case 0:
                case 1:Cartridge.mirroring = Mirroring.Single;break;
                case 2: Cartridge.mirroring = Mirroring.Horisontal;break;
                case 3: Cartridge.mirroring = Mirroring.Vertical; break;
            } 
            switch ((val >> 2) & 3) 
            { 
                case 0:
                case 1: prg_mode = PRGMode.Switch32; break;
                case 2: prg_mode = PRGMode.FixFirst; break;
                case 3: prg_mode = PRGMode.FixLast; break;
            }
          
            chr_mode = (CHRmode)((val >> 4) & 1);
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
