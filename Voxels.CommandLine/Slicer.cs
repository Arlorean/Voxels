using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Voxels.CommandLine {
    public class Slicer {
        /// <summary>
        /// Render each slice of the voxel data into a long single PNG file for use as a 3D texture in Unity.
        /// Unity 3D textures are sliced along the Unity Z axis, which is the Voxel Y axis (front to back).
        /// </summary>
        /// <param name="voxels"></param>
        /// <returns>A byte array of the voxel slices in PNG format.</returns>
        public static byte[] RenderTexture3D(VoxelData voxels, int columns, int rows) {
            using (var bitmap = new SKBitmap(voxels.Size.X*columns, voxels.Size.Z*rows)) {
                if (bitmap == null) return null;

                for (var y = 0; y < voxels.Size.Y; y++) {
                    var dx = (y%columns) * voxels.Size.X;
                    var dz = (y/columns) * voxels.Size.Z;
                    for (var x = 0; x < voxels.Size.X; x++) {
                        for (var z = 0; z < voxels.Size.Z; z++) {
                            var c = voxels.ColorOf(new XYZ(x, y, (voxels.Size.Z-z-1)));
                            bitmap.SetPixel(x + dx, z + dz, new SKColor(c.R, c.G, c.B, c.A));
                        }
                    }
                }

                using (var image = SKImage.FromBitmap(bitmap)) {
                    using (var data = image.Encode()) {
                        var ms = new MemoryStream();
                        data.SaveTo(ms);
                        return ms.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Unity does not like really wide textures so we have to create the most square texture we can.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        public static void MakeSquare(XYZ size, out int rows, out int columns) {
            rows = 1;
            columns = size.Y;

            while (columns % 2 == 0) {
                columns /= 2;
                rows *= 2;

                // Closest we can get to a square
                if (columns <= rows) {
                    return;
                }
            }
        }
    }
}
