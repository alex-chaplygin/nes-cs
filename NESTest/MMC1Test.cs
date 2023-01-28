﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NES;

namespace NESTest
{
    [TestClass]
    public class MMC1Test
    {
       
        public void Write()
        {
            Cartridge.mapper = 1;
            Memory.Write(0xE000, 0x80);
            byte a = 0xF;
            for (int i = 0; i < 5; i++) 
            {
                Memory.Write(0xE000, a);
                a >>= 1;
            }
            Assert.AreEqual(0xF, MMC1.registr);
        }

        
       public MMC1Test() 
       {
            Cartridge.mapper = 1;
            Cartridge.prg_count = 3;
            Cartridge.prg_mem = new byte[Memory.PRG_SIZE * 3];
            for (int i= 0;i<Cartridge.prg_mem.Length; i++)
            {
                if (i < Memory.PRG_SIZE)
                    Cartridge.prg_mem[i] = 1;
               else if (i < Memory.PRG_SIZE * 2)
                    Cartridge.prg_mem[i] = 2;
                else
                    Cartridge.prg_mem[i] = 3;
                
            }

            Cartridge.chr_count = 4;
            Cartridge.chr_mem = new byte[Memory.CHR_SIZE_4BYTES*4];
            for(int i=0; i < Cartridge.chr_mem.Length; i++)
            {
                if (i < Memory.CHR_SIZE_4BYTES)
                    Cartridge.chr_mem[i] = 1;
                else if (i < Memory.CHR_SIZE_4BYTES * 2)
                    Cartridge.chr_mem[i] = 2;
                else if (i < Memory.CHR_SIZE_4BYTES * 3)
                    Cartridge.chr_mem[i] = 3;
                else
                    Cartridge.chr_mem[i] = 4;  
            }
       }
        public void Switch(ushort adr, byte bank, byte load)
        {
            //Установка режимов переключения и зеркалирования
            Memory.Write(0x8000, load);
            byte a = bank;
            for (int i = 0; i < 5; i++)
            {
                Memory.Write(adr, a);
                a >>= 1;
            }
        }
       

        /// <summary>
        /// Сброс MMC1
        /// </summary>
        [TestMethod]
        public void ResetMMC1()
        {
            Memory.Write(0x8000,0x80);
            Assert.AreEqual(3, Memory.Read(0xC000));
        }
        /// <summary>
        /// Тест переключений PRG
        /// </summary>
       [TestMethod] 
        public void TestSwitchPRG()
        {
            //Переключение
           Switch(0xE000, 0, 4);
            Assert.AreEqual(1, Memory.Read(0x8000));
            Assert.AreEqual(2, Memory.Read(0xC000));
           
           Switch(0xE000, 1, 8);
            Assert.AreEqual(1,Memory.Read(0x8000));
            Assert.AreEqual(2, Memory.Read(0xC000));

           Switch(0xE000, 2, 12);
            Assert.AreEqual(3, Memory.Read(0x8000));
            Assert.AreEqual(3, Memory.Read(0xC000));

        }
        [TestMethod]
        public void TestSwitchCHR()
        {
            Switch(0xA000, 0, 0);
            Memory.Write(0x2006, 0);
            Memory.Write(0x2006, 0);
            Memory.Read(0x2007);
            Assert.AreEqual(1, Memory.Read(0x2007));
            Memory.Write(0x2006, 0x10);
            Memory.Write(0x2006, 0);
            Memory.Read(0x2007);
            Assert.AreEqual(2, Memory.Read(0x2007));

            Switch(0xA000, 2, 0);
            Memory.Write(0x2006, 0);
            Memory.Write(0x2006, 0);
            Memory.Read(0x2007);
            Assert.AreEqual(3, Memory.Read(0x2007));
            Memory.Write(0x2006, 0x10);
            Memory.Write(0x2006, 0);
            Memory.Read(0x2007);
            Assert.AreEqual(4, Memory.Read(0x2007));

            Switch(0xA000, 2, 16);
            Memory.Write(0x2006, 0);
            Memory.Write(0x2006, 0);
            Memory.Read(0x2007);
            Assert.AreEqual(3, Memory.Read(0x2007));

            Switch(0xC000, 3, 16);
            Memory.Write(0x2006, 0);
            Memory.Write(0x2006, 0);
            Memory.Read(0x2007);
            Assert.AreEqual(3, Memory.Read(0x2007));
            Memory.Write(0x2006, 0x10);
            Memory.Write(0x2006, 0);
            Memory.Read(0x2007);
            Assert.AreEqual(4, Memory.Read(0x2007));
        }
            
    }
}
