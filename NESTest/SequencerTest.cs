using Microsoft.VisualStudio.TestTools.UnitTesting;
using NES;

namespace NESTest
{
    [TestClass]
    public class SequencerTest
    {        
        [TestMethod]
        public void TestMethod()
        {
            int[] a = { 0, 1, 2, 3, 4 };
            Sequencer b = new(a);
            int c;

            c = b.Clock();
            Assert.AreEqual(0, c);

            c = b.Clock();
            Assert.AreEqual(1, c);

            c = b.Clock();
            Assert.AreEqual(2, c);

            c = b.Clock();
            Assert.AreEqual(3, c);

            c = b.Clock();
            Assert.AreEqual(4, c);

            c = b.Clock();
            Assert.AreEqual(0, c);
        }
    }
}
