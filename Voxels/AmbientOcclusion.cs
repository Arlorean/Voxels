using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxels {
    public class AmbientOcclusion {
        /// <summary>
        /// Calculate simple ambient for voxel based on surrounding voxels.
        /// https://0fps.net/2013/07/03/ambient-occlusion-for-minecraft-like-worlds/
        /// </summary>
        /// <param name="voxelData">voxel data set.</param>
        /// <param name="p">The voxel being rendered.</param>
        /// <param name="left">The voxel to the left side.</param>
        /// <param name="right">The voxel to the right side.</param>
        /// <param name="up">The voxel above.</param>
        /// <returns>An integer from 0-3 representing the occlusion case.</returns>
        public static int CalculateAO(VoxelData voxelData, XYZ p, XYZ left, XYZ right, XYZ up) {
            var side1 = Math.Sign(voxelData[p + left + up].colorIndex); // 0 or 1
            var side2 = Math.Sign(voxelData[p + right + up].colorIndex); // 0 or 1
            var corner = Math.Sign(voxelData[p + left + right + up].colorIndex); // 0 or 1

            if (side1 == 1 && side2 == 1) {
                return 0;
            }
            return 3 - (side1 + side2 + corner);
        }
    }
}
