using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Voxels.Test {
    [TestClass]
    public class TestAmbientOcclusion {

        [TestMethod]
        public void TestCase0() {
            using (var stream = File.OpenRead("3x3x3.vox")) {
                var voxelData = MagicaVoxel.Read(stream);
                Assert.AreEqual(0, AmbientOcclusion.CalculateAO(voxelData, new XYZ(1, 1, 1), XYZ.OneY, XYZ.OneX, XYZ.OneZ));
            }
        }

        [TestMethod]
        public void TestCase1() {
            using (var stream = File.OpenRead("3x3x3.vox")) {
                var voxelData = MagicaVoxel.Read(stream);
                Assert.AreEqual(1, AmbientOcclusion.CalculateAO(voxelData, new XYZ(1, 0, 1), XYZ.OneY, XYZ.OneX, XYZ.OneZ));
                Assert.AreEqual(1, AmbientOcclusion.CalculateAO(voxelData, new XYZ(0, 1, 1), XYZ.OneY, XYZ.OneX, XYZ.OneZ));
            }
        }
        [TestMethod]
        public void TestCase2() {
            using (var stream = File.OpenRead("3x3x3.vox")) {
                var voxelData = MagicaVoxel.Read(stream);
                Assert.AreEqual(2, AmbientOcclusion.CalculateAO(voxelData, new XYZ(1, 1, 0), XYZ.OneY, XYZ.OneX, XYZ.OneZ));
            }
        }
        [TestMethod]
        public void TestCase3() {
            using (var stream = File.OpenRead("3x3x3.vox")) {
                var voxelData = MagicaVoxel.Read(stream);
                Assert.AreEqual(3, AmbientOcclusion.CalculateAO(voxelData, new XYZ(2, 2, 2), XYZ.OneY, XYZ.OneX, XYZ.OneZ));
            }
        }
    }
}
