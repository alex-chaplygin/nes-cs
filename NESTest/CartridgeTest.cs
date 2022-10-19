using NES;

namespace NESTest {
    [TestClass]
    public class CartridgeTest {
        void Test(string fileName, byte[] actualChrMem, byte[] actualTrainerMem) {
            Cartridge.ReadFile(fileName);
            Assert.AreEqual(Cartridge.chr_mem, actualChrMem);
            Assert.AreEqual(Cartridge.trainer_mem, actualTrainerMem);
        }

        [TestMethod]
        public void TetrisTest()
        {
            Cartridge.ReadFile(@"..\..\..\Tetris.nes");
        }

        [TestMethod]
        public void CastlevaniaTest()
        {
            Cartridge.ReadFile(@"..\..\..\Castlevania.nes");
        }

        [TestMethod]
        public void SMBTest()
        {
            Cartridge.ReadFile(@"..\..\..\Super_mario_brothers.nes");
        }
    }
}
