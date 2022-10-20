using NES;

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
        public void ReadWordTest()
        {
	    Memory.memory[0x100] = 0xAA;
            Memory.memory[0x101] = 0xBB;
            Assert.AreEqual(0xBBAA, Memory.ReadWord(0x100));
        }
    }
}
