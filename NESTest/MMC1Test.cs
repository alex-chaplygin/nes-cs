using Microsoft.VisualStudio.TestTools.UnitTesting;
using NES;

namespace NESTest
{
    [TestClass]
    public class MMC1Test
    {
        
        [TestMethod]
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
    }
}
