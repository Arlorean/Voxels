using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxels.CommandLine {
    internal static class ImageToVoxel {
        public static VoxelData Import(string path, Color[] palette) {
            using (var bitmap = SKBitmap.Decode(path)) {
                if (bitmap == null) return null;

                var size = new XYZ(bitmap.Width, 1, bitmap.Height);
                var voxelData = new VoxelData(size, palette);

                for (var x = 0; x < bitmap.Width; x++) {
                    for (var y = 0; y < bitmap.Height; y++) {
                        var p = bitmap.GetPixel(x, y);
                        if (p.Alpha != 0) {
                            var c = new Color(p.Red, p.Green, p.Blue, p.Alpha);
                            var i = Array.IndexOf(palette, c);
                            voxelData[new XYZ(x, 0, bitmap.Height-y-1)] = new Voxel((uint)i);
                        }
                    }
                }

                return voxelData;
            }
        }

        public static void ExtractColors(string path, HashSet<Color> colorsUsed) {
            using (var bitmap = SKBitmap.Decode(path)) {
                if (bitmap == null) return;

                for (var x = 0; x < bitmap.Width; x++) {
                    for (var y = 0; y < bitmap.Height; y++) {
                        var p = bitmap.GetPixel(x, y);
                        if (p.Alpha != 0) {
                            var c = new Color(p.Red, p.Green, p.Blue, p.Alpha);
                            colorsUsed.Add(c);
                        }
                    }
                }
            }
        }
    }
}
