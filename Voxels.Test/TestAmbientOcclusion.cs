using System;
using NUnit.Framework;
using System.IO;

namespace Voxels.Test {
    [TestFixture]
    public class TestAmbientOcclusion {
        [OneTimeSetUp]
        public void SetUp() {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [Test]
        public void TestCase0() {
            using (var stream = File.OpenRead("3x3x3.vox")) {
                var voxelData = MagicaVoxel.Read(stream);
                Assert.AreEqual(0, AmbientOcclusion.CalculateAO(voxelData, new XYZ(1, 1, 1), XYZ.OneY, XYZ.OneX, XYZ.OneZ));
            }
        }

        [Test]
        public void TestCase1() {
            using (var stream = File.OpenRead("3x3x3.vox")) {
                var voxelData = MagicaVoxel.Read(stream);
                Assert.AreEqual(1, AmbientOcclusion.CalculateAO(voxelData, new XYZ(1, 0, 1), XYZ.OneY, XYZ.OneX, XYZ.OneZ));
                Assert.AreEqual(1, AmbientOcclusion.CalculateAO(voxelData, new XYZ(0, 1, 1), XYZ.OneY, XYZ.OneX, XYZ.OneZ));
            }
        }
        [Test]
        public void TestCase2() {
            using (var stream = File.OpenRead("3x3x3.vox")) {
                var voxelData = MagicaVoxel.Read(stream);
                Assert.AreEqual(2, AmbientOcclusion.CalculateAO(voxelData, new XYZ(1, 1, 0), XYZ.OneY, XYZ.OneX, XYZ.OneZ));
            }
        }
        [Test]
        public void TestCase3() {
            using (var stream = File.OpenRead("3x3x3.vox")) {
                var voxelData = MagicaVoxel.Read(stream);
                Assert.AreEqual(3, AmbientOcclusion.CalculateAO(voxelData, new XYZ(2, 2, 2), XYZ.OneY, XYZ.OneX, XYZ.OneZ));
            }
        }
        [Test]
        public void TestCaseComplex() {
            using (var stream = File.OpenRead("2x2x2.vox")) {
                var voxelData = MagicaVoxel.Read(stream);
                Assert.AreEqual(0, AmbientOcclusion.CalculateAO(voxelData, new XYZ(0, 0, 0), XYZ.OneY, XYZ.OneX, XYZ.OneZ));
                Assert.AreEqual(0, AmbientOcclusion.CalculateAO(voxelData, new XYZ(0, 1, 1), XYZ.OneX, -XYZ.OneZ, -XYZ.OneY));
                Assert.AreEqual(0, AmbientOcclusion.CalculateAO(voxelData, new XYZ(1, 0, 1), -XYZ.OneZ, XYZ.OneY, -XYZ.OneX));
                Assert.AreEqual(2, AmbientOcclusion.CalculateAO(voxelData, new XYZ(0, 0, 0), -XYZ.OneX, XYZ.OneY, XYZ.OneZ));
                Assert.AreEqual(2, AmbientOcclusion.CalculateAO(voxelData, new XYZ(0, 0, 0), XYZ.OneX, -XYZ.OneY, XYZ.OneZ));
                Assert.AreEqual(3, AmbientOcclusion.CalculateAO(voxelData, new XYZ(0, 0, 0), -XYZ.OneX, -XYZ.OneY, XYZ.OneZ));
            }
        }

    }
}
