using System;
using NUnit.Framework;
using System.IO;

namespace Voxels.Test {
    [TestFixture]
    public class TestQubicle {
        [OneTimeSetUp]
        public void SetUp() {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        /// <summary>
        /// The Nerds.qb is the test file that comes with the Qubicle demo.
        /// </summary>
        [Test]
        public void TestNerds() {
            using (var stream = File.OpenRead("Nerds.qb")) {
                var qubicle = new QbFile();
                qubicle.Read(stream);

                Assert.AreEqual(257, qubicle.Version);

                var voxelData = qubicle.Flatten();

                Assert.AreEqual(new XYZ(48,45,44), voxelData.Size);

                Assert.AreEqual(15965, voxelData.Count);

                // Left size speaker foot
                Assert.AreEqual(new Color(89, 93, 96), voxelData.ColorOf(XYZ.Zero));

                // Middle nerd's hair
                Assert.AreEqual(new Color(83, 33, 21), voxelData.ColorOf(new XYZ(24, 40, 43)));
            }
        }
    }
}
