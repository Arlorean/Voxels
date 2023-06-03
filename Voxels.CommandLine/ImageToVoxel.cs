using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxels.CommandLine {
    internal static class ImageToVoxel {
        public static VoxelData Import(string path) {
            using (var bitmap = SKBitmap.Decode(path)) {
                if (bitmap == null) return null;

                var size = new XYZ(bitmap.Width, 1, bitmap.Height);
                var voxelData = new VoxelData(size);

                for (var x = 0; x < bitmap.Width; x++) {
                    for (var y = 0; y < bitmap.Height; y++) {
                        var p = bitmap.GetPixel(x, y);
                        if (p.Alpha != 0) {
                            var c = new Color(p.Red, p.Green, p.Blue, p.Alpha);
                            voxelData[new XYZ(x, 0, y)] = new Voxel(c);
                        }
                    }
                }

                return voxelData;
            }
        }
    }
}
