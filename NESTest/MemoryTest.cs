using NES;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NESTest
{
    [TestClass]
    public class MemoryTest
    {
        void Test(ushort a1, ushort a2)
        {
            Memory.memory[a1] = 0xAA;
            Assert.AreEqual(0xAA, Memory.Read(a2));
        }

        [TestMethod]
        public void ReadTest()
        {
            Test(5, 0x805);
        }

        [TestMethod]
        public void ReadTest2()
        {
            Test(0x7FF, 0x17FF);
        }

        [TestMethod]
        public void ReadTest3()
        {
            Test(0xA, 0x180A);
        }

	[TestMethod]
        public void ReadTest4()
        {
            Test(0x5333, 0x5333);
        }
        
        [TestMethod]
        public void ReadTest5()
        {
            Test(0x8000, 0x8000);
        }

	[TestMethod]
        public void WriteTest()
        {
            Memory.Write(0x800, 5);           
            Assert.AreEqual(5, Memory.Read(0x800));
        }
	
	[TestMethod]
        public void ReadWordTest()
        {
	    Memory.memory[0x100] = 0xAA;
            Memory.memory[0x101] = 0xBB;
            Assert.AreEqual(0xBBAA, Memory.ReadWord(0x100));
        }

	[TestMethod]
        public void WriteROM1Test()
        {
            byte[] G = new byte[] { 0xA, 0xB, 0xC, 0xE };
            Memory.WriteROM1(G);

            Assert.AreEqual(0xA, Memory.memory[0x8000]);
            Assert.AreEqual(0xB, Memory.memory[0x8001]);
            Assert.AreEqual(0xC, Memory.memory[0x8002]);
            Assert.AreEqual(0xE, Memory.memory[0x8003]);

        }
	
        [TestMethod]
        public void WriteROM1Test2()
        {
            byte[] g = new byte[Memory.PRG_SIZE];
            for (int y = 0; y < Memory.PRG_SIZE; y++)
                g[y] = 0x56;
            Memory.WriteROM1(g);
            Assert.AreEqual(0x56, Memory.memory[0xC000 - 1]);
        }

        [TestMethod]
        public void WriteROM2Test()
        {
            byte[] G = new byte[] { 0xA, 0xB, 0xC, 0xE };
            Memory.WriteROM2(G);
            Assert.AreEqual(0xA, Memory.memory[0xC000]);
            Assert.AreEqual(0xB, Memory.memory[0xC000 + 1]);
            Assert.AreEqual(0xC, Memory.memory[0xC000 + 2]);
            Assert.AreEqual(0xE, Memory.memory[0xC000 + 3]);
        }

        [TestMethod]
        public void WriteROM2Test2()
        {
            byte[] g = new byte[Memory.PRG_SIZE];
            for (int y = 0; y < Memory.PRG_SIZE; y++)
                g[y] = 0x5;
            Memory.WriteROM2(g);
            Assert.AreEqual(0x5, Memory.memory[0xFFFF]);
        }
    }
}
