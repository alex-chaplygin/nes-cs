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
            Assert.AreEqual(false, counter.Clock());
            Assert.AreEqual(4,counter.GetValue());
            Assert.AreEqual(false,counter.Clock());
            Assert.AreEqual(false, counter.Clock());
            Assert.AreEqual(false, counter.Clock());
            Assert.AreEqual(true, counter.Clock());
            Assert.AreEqual(0, counter.GetValue());
        }
    }
}
