using NES;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NESTest
{
    [TestClass]
    public class LengthCounterTest
    {
        [TestMethod] 
        public void ClockTest()
        {
            NES.APU.LengthCounter counter = new NES.APU.LengthCounter(5);
            Assert.AreEqual(4,counter.GetValue());
            Assert.AreEqual(false,counter.Clock());
            Assert.AreEqual(false, counter.Clock());
            Assert.AreEqual(false, counter.Clock());
            Assert.AreEqual(true, counter.Clock());
            Assert.AreEqual(0, counter.GetValue());
        }

	[TestMethod] 
        public void GetValueTest()
        {
            NES.APU.LengthCounter counter = new NES.APU.LengthCounter(1);;
            Assert.AreEqual(254,counter.GetValue());
            counter.Set(9);
            Assert.AreEqual(8, counter.GetValue());
            counter.Set(0x1B);
            Assert.AreEqual(26, counter.GetValue());
            counter.Set(0x19);
            Assert.AreEqual(24, counter.GetValue());
            counter.Set(0x18);
            Assert.AreEqual(192, counter.GetValue());
        }
    }
}
