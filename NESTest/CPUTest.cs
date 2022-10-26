using NES;

namespace NESTest
{
    [TestClass]
    public class CPUTest
    {
        [TestMethod]
        public void StackTest()
        {
            CPU.SP = 0xFF;
            CPU.Push(0x11);
            Assert.AreEqual(0x11, Memory.memory[0x1FF]);
            Assert.AreEqual(0xFE, CPU.SP);
        }

        [TestMethod]
        public void StackTest2()
        {
            CPU.SP = 0xFF;
            CPU.Push(0x11);
            Assert.AreEqual(0x11, CPU.Pop());
            Assert.AreEqual(0xFF, CPU.SP);
        }

	[TestMethod]
        public void StackTest3()
        {
            CPU.SP = 0xFF;
            CPU.PushWord(0x1234);
            //CPU.Push(0x34); CPU.Push(0x12);
            Assert.AreEqual(0x34, Memory.memory[0x1FF]);
            Assert.AreEqual(0x12, Memory.memory[0x1FE]);
            Assert.AreEqual(0x1234, CPU.ToWord(Memory.memory[0x1FE], Memory.memory[0x1FF]));
            Assert.AreEqual(0xFD, CPU.SP);
        }

	[TestMethod]
        public void StackTest4()
        {
            CPU.SP = 0xFD;
            Assert.AreEqual(CPU.PopWord(), CPU.ToWord(Memory.memory[0x1FE], Memory.memory[0x1FF]));
            Assert.AreEqual(0xFF, CPU.SP);
        }
    }
}
