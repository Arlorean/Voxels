using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxels.Test {
    [TestFixture]

    public class TestColor {
        [Test]
        public void TestHSV() {
            float h, s, v;

            Color.Black.ToHSV(out h, out s, out v);
            Assert.AreEqual(0, h);
            Assert.AreEqual(0, s);
            Assert.AreEqual(0, v);
            Assert.AreEqual(Color.Black, Color.FromHSV(h, s, v));

            Color.White.ToHSV(out h, out s, out v);
            Assert.AreEqual(0, h);
            Assert.AreEqual(0, s);
            Assert.AreEqual(1, v);
            Assert.AreEqual(Color.White, Color.FromHSV(h, s, v));

            Color.Red.ToHSV(out h, out s, out v);
            Assert.AreEqual(0, h);
            Assert.AreEqual(1, s);
            Assert.AreEqual(1, v);
            Assert.AreEqual(Color.Red, Color.FromHSV(h, s, v));

            Color.Green.ToHSV(out h, out s, out v);
            Assert.AreEqual(120, h);
            Assert.AreEqual(1, s);
            Assert.AreEqual(1, v);
            Assert.AreEqual(Color.Green, Color.FromHSV(h, s, v));

            Color.Blue.ToHSV(out h, out s, out v);
            Assert.AreEqual(240, h);
            Assert.AreEqual(1, s);
            Assert.AreEqual(1, v);
            Assert.AreEqual(Color.Blue, Color.FromHSV(h, s, v));

            Color.Tan.ToHSV(out h, out s, out v);
            Assert.AreEqual(34, h);
            Assert.AreEqual(0.3333333, s, 1e-6);
            Assert.AreEqual(0.8235294, v, 1e-6);
            Assert.AreEqual(Color.Tan, Color.FromHSV(h, s, v));
        }
    }
}
