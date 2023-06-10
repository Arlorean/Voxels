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
                var magicaVoxel = new MagicaVoxel();
                magicaVoxel.Read(stream);

                var voxelData = magicaVoxel.Flatten();
                Assert.AreEqual(6, voxelData.Count);
                Assert.AreEqual(1, voxelData.Size.X);
                Assert.AreEqual(2, voxelData.Size.Y);
                Assert.AreEqual(3, voxelData.Size.Z);
            }
        }

        [Test]
        public void Test3x3x3() {
            using (var stream = File.OpenRead("3x3x3.vox")) {
                var magicaVoxel = new MagicaVoxel();
                magicaVoxel.Read(stream);

                var voxelData = magicaVoxel.Flatten();
                Assert.AreEqual(20, voxelData.Count);
            }
        }

        [Test]
        public void Test8x8x8() {
            using (var stream = File.OpenRead("8x8x8.vox")) {
                var magicaVoxel = new MagicaVoxel();
                magicaVoxel.Read(stream);

                var voxelData = magicaVoxel.Flatten();
                Assert.AreEqual(80, voxelData.Count);
            }
        }

        /// <summary>
        /// The cars.vox is the MagicaVoxel example of using multiple voxel models
        /// with rotations and translations to place them in a larger world.
        /// </summary>
        [Test]
        public void TestCars() {
            using (var stream = File.OpenRead("cars.vox")) {
                var magicaVoxel = new MagicaVoxel();
                magicaVoxel.Read(stream);

                Assert.AreEqual(200, magicaVoxel.Version);
                Assert.AreEqual(2, magicaVoxel.Models.Count);

                Assert.AreEqual(3132, magicaVoxel.Models[0].Count);
                Assert.AreEqual(3235, magicaVoxel.Models[1].Count);

                Assert.AreEqual(32, magicaVoxel.Layers.Count); // Not sure how this got created since the default layer count is 16

                Assert.AreEqual(3132 + 3235, magicaVoxel.Flatten().Count);
            }
        }
    }
}
