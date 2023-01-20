using NES;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NESTest
{
    [TestClass]
    public class PPUTest
    {
        [TestMethod]
        public void SetAddressTest()
        {
            PPU.SetAddress(0xFF);
            PPU.SetAddress(0x7F);
            Assert.AreEqual(0xFF7F, PPU.address);
            PPU.SetAddress(0x7F);
            PPU.SetAddress(0x37);
            Assert.AreEqual(0x7F37, PPU.address);
            PPU.SetAddress(0x00);
            PPU.SetAddress(0x00);
            Assert.AreEqual(0x00, PPU.address);
        }

	[TestMethod]
        public void WritePatternTest0()
        {
            byte[] data = new byte[0x1000];
            for (int i = 0; i<0x1000; i++)
            {
                data[i] = 1;
            }
            PPU.WritePattern0(data);
            Assert.AreEqual(1, PPU.memory[0x1000 - 1]);
            //Assert.AreEqual(0, PPU.memory[0x1000]);
        }

	[TestMethod]
        public void WritePatternTest1()
        {
            byte[] data = new byte[0x1000];
            for (int i = 0; i < 0x1000; i++)
            {
                data[i] = 2;
            }
            PPU.WritePattern1(data);
            Assert.AreEqual(2, PPU.memory[0x1000]);
            Assert.AreEqual(2, PPU.memory[0x1400]);
        }
	
        [TestMethod]
        public void Test_8Kb()
        {
            byte[] data = new byte[0x2000];
            for (int i = 0; i < 0x2000; i++)
            {
                data[i] = 3;
            }
            PPU.WritePattern0(data);
            Assert.AreEqual(3, PPU.memory[0x2000 - 1]);
        }

	[TestMethod]
	public void Test1_WriteData()
	{
	    PPU.increment = 0;
	    PPU.DataWrite(5);
	    Assert.AreEqual(1, PPU.address);
	}
	
	[TestMethod]
	public void Test2_WriteData()
	{
	    PPU.increment = 1;
	    PPU.DataWrite(5);
	    Assert.AreEqual(32, PPU.address) ;
	}

	[TestMethod]
        public void ReadTest()
        {
            try
            {
                Memory.Read(0x2006);
            } catch (Exception)
            {
                return;
            }
            //Assert.Fail();
        }

        [TestMethod]
        public void MirrorTest()
        {
	    Cartridge.mirroring = Mirroring.Horisontal;
            ushort adr = 0x2400;
            
            Assert.AreEqual(0x2000, PPU.MirroringGet(adr));
            Assert.AreEqual(0x1000, PPU.MirroringGet(0x1000));
            Assert.AreEqual(0x23FF, PPU.MirroringGet(0x23FF));
            Assert.AreEqual(0x23FF, PPU.MirroringGet(0x27FF));
            Assert.AreEqual(0x2BFF, PPU.MirroringGet(0x2FFF));
            Assert.AreEqual(0x3F00, PPU.MirroringGet(0x3F00));

            Cartridge.mirroring = Mirroring.Vertical;

            Assert.AreEqual(0x2400, PPU.MirroringGet(0x2C00));
            Assert.AreEqual(0x23FF, PPU.MirroringGet(0x2BFF));

            Assert.AreEqual(0x3F00, PPU.MirroringGet(0x3F10));
            Assert.AreEqual(0x3F04, PPU.MirroringGet(0x3F14));
            Assert.AreEqual(0x3F08, PPU.MirroringGet(0x3F18));
            Assert.AreEqual(0x3F0C, PPU.MirroringGet(0x3F1C));

            Cartridge.mirroring = Mirroring.Single;

            Assert.AreEqual(0x2000, PPU.MirroringGet(0x2400));
            Assert.AreEqual(0x21F7, PPU.MirroringGet(0x29F7));
            Assert.AreEqual(0x23FF, PPU.MirroringGet(0x2FFF));
        }

	[TestMethod]
        public void Test_ReadData()
        {
            PPU.increment = 0;
            PPU.WriteData(1);
            PPU.WriteData(3);
            PPU.WriteData(2);
            PPU.address = 0;
            Assert.AreEqual(0, PPU.ReadData());
            Assert.AreEqual(1, PPU.ReadData());
            Assert.AreEqual(3, PPU.ReadData());
            Assert.AreEqual(2, PPU.ReadData());
        }

	[TestMethod]
        public void GetTileAdrTest()
        {
            Assert.AreEqual(0, PPU.GetTileAdr(0, 0, 0));
            Assert.AreEqual(0x13, PPU.GetTileAdr(1, 3, 0));
            Assert.AreEqual(0x1030, PPU.GetTileAdr(3, 0, 1));
            Assert.AreEqual(0x1025, PPU.GetTileAdr(2, 5, 1));
        }

	[TestMethod]
        public void Test_StatusRead()
        {
            PPU.sprite_overflow = true;
            PPU.sprite_0_hit = false;
            PPU.vertical_blank = true;
            Assert.AreEqual(0b_1010_0000, PPU.StatusRead());
            PPU.sprite_overflow = false;
            PPU.sprite_0_hit = true;
            PPU.vertical_blank = true;
            Assert.AreEqual(0b_1100_0000, PPU.StatusRead());
            PPU.sprite_overflow = true;
            PPU.sprite_0_hit = true;
            PPU.vertical_blank = false;
            Assert.AreEqual(0b_0110_0000, PPU.StatusRead());
        }

	[TestMethod]
        public void DMATest()
        {
            for (int i = 0; i < 256; i++)
            {
                Memory.Write((ushort)((0x02 << 8) + i), (byte)i);
            }
            PPU.OAM_address = 1;
            PPU.DMA(0x02);
            Assert.AreEqual(PPU.OAM_memory[0], 255);
            Assert.AreEqual(PPU.OAM_memory[64], 63);
            Assert.AreEqual(PPU.OAM_memory[255], 254);
        }
    }
}
