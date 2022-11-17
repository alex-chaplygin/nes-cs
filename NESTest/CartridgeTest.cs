using NES;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NESTest {
    [TestClass]
    public class CartridgeTest {
        void Test(string fileName, byte[] expectedChrMem, byte[] expectedTrainerMem) {
            Cartridge.ReadFile(fileName);
            byte[] chrMem = Cartridge.chr_mem.Take(16).ToArray();
            byte[] trainerMem = Cartridge.trainer_mem.Take(16).ToArray();
            CollectionAssert.AreEquivalent(expectedChrMem, chrMem);
            CollectionAssert.AreEquivalent(expectedTrainerMem, trainerMem);
        }

        /*[TestMethod]
        public void TetrisTest() {
            byte[] chrMem = {};
            byte[] trainerMem = {};
            Test(@"..\..\..\Tetris.nes", chrMem, trainerMem);
        }*/

        [TestMethod]
        public void CastlevaniaTest() {
            byte[] chrMem = {};
            byte[] trainerMem = {};
            Test(@"..\..\..\Castlevania.nes", chrMem, trainerMem);
        }

        [TestMethod]
        public void SMBTest()
        {
            byte[] chrMem = {0x03, 0x0F, 0x1F, 0x1F, 0x1C, 0x24, 0x26, 0x66, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x3F, 0x3F, 0x7F};
            byte[] trainerMem = {};
            Test(@"..\..\..\Super_mario_brothers.nes", chrMem, trainerMem);
        }

        [TestMethod]
        public void CastlevaniaCHRTest() {
            Cartridge.ReadFile(@"..\..\..\Castlevania.nes");
            Assert.IsNull(Cartridge.GetChrBank(0));
            Assert.IsNull(Cartridge.GetChrBank(1));
            Assert.IsNull(Cartridge.GetChrBank(2));
        }
    }
}
