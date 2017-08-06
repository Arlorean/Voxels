using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Voxels.SkiaSharp;

namespace Voxels.CommandLine {
    class Program {
        static void Main(string[] args) {
            var filename = args.Length == 1 ? args[0] : "wizard.vox";
            using (var stream = File.OpenRead(filename)) {
                var voxelData = MagicaVoxel.Read(stream);

                File.WriteAllBytes(Path.ChangeExtension(filename, ".png"), Renderer.RenderPng(512, voxelData));
                File.WriteAllBytes(Path.ChangeExtension(filename, ".svg"), Renderer.RenderSvg(512, voxelData));
            }
        }
    }
}
