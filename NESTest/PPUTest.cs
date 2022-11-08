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
            Assert.AreEqual(1, PPU.ppu_memory[0x1000 - 1]);
            //Assert.AreEqual(0, PPU.ppu_memory[0x1000]);
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
            Assert.AreEqual(2, PPU.ppu_memory[0x1000]);
            Assert.AreEqual(2, PPU.ppu_memory[0x1400]);
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
            Assert.AreEqual(3, PPU.ppu_memory[0x2000 - 1]);
        }
    }
}
