using NES;

namespace NESTest {
    [TestClass]
    public class CartridgeTest {
        void Test(string fileName, byte[] expectedChrMem, byte[] expectedTrainerMem) {
            Cartridge.ReadFile(fileName);
            byte[] chrMem = Cartridge.chr_mem.Take(16).ToArray();
            byte[] trainerMem = Cartridge.trainer_mem.Take(16).ToArray();
            Assert.AreEqual(expectedChrMem, chrMem);
            Assert.AreEqual(expectedTrainerMem, trainerMem);
        }

        [TestMethod]
        public void TetrisTest()
        {
            byte[] chrMem = {0x38, 0x4c, 0xc6, 0xc6, 0xc6, 0x64, 0x38, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0};
            byte[] trainerMem = {};
            Test(@"..\..\..\Tetris.nes", chrMem, trainerMem);
        }

        [TestMethod]
        public void CastlevaniaTest()
        {
            byte[] chrMem = {};
            byte[] trainerMem = {};
            Test(@"..\..\..\Castlevania.nes", chrMem, trainerMem);
        }

        [TestMethod]
        public void SMBTest()
        {
            byte[] chrMem = {};
            byte[] trainerMem = {};
            Test(@"..\..\..\Super_mario_brothers.nes", chrMem, trainerMem);
        }
    }
}
