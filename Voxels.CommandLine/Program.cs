using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Voxels.SkiaSharp;

namespace Voxels.CommandLine {
    class Program {
        static void Main(string[] args) {
            var filename = args.Length == 1 ? args[0] : "monu9.vox"; // "3x3x3.vox";
            using (var stream = File.OpenRead(filename)) {
                var voxelData = MagicaVoxel.Read(stream);

                var guid = Guid.NewGuid();
                File.WriteAllBytes($"output{guid}.png", Renderer.RenderPng(512, voxelData));
                File.WriteAllBytes($"output{guid}.svg", Renderer.RenderSvg(512, voxelData));
            }
        }
    }
}
