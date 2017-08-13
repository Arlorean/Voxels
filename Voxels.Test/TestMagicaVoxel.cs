using System;
using NUnit.Framework;
using System.IO;

namespace Voxels.Test {
    [TestFixture]
    public class TestMagicaVoxel {
        [OneTimeSetUp]
        public void SetUp() {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [Test]
        public void Test1x2x3() {
            using (var stream = File.OpenRead("1x2x3.vox")) {
                var voxelData = MagicaVoxel.Read(stream);
                Assert.AreEqual(6, voxelData.Count);
                Assert.AreEqual(1, voxelData.size.X);
                Assert.AreEqual(2, voxelData.size.Y);
                Assert.AreEqual(3, voxelData.size.Z);
            }
        }

        [Test]
        public void Test3x3x3() {
            using (var stream = File.OpenRead("3x3x3.vox")) {
                var voxelData = MagicaVoxel.Read(stream);
                Assert.AreEqual(20, voxelData.Count);
            }
        }

        [Test]
        public void Test8x8x8() {
            using (var stream = File.OpenRead("8x8x8.vox")) {
                var voxelData = MagicaVoxel.Read(stream);
                Assert.AreEqual(80, voxelData.Count);
            }
        }
    }
}
