using System;
using System.IO;
using Voxels.SkiaSharp;

namespace Voxels.CommandLine {
    class Program {
        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("usage: Voxels.CommandLine.exe voxfiles...\n      Converts .vox files to .png and .svg files.");
            }
            else {
                NativeLibrary.Initialize();

                foreach (var filename in args) {
                    var voxelData = VoxelImport.Import(filename);

                    File.WriteAllBytes(Path.ChangeExtension(filename, ".png"), Renderer.RenderPng(512, voxelData));
                    File.WriteAllBytes(Path.ChangeExtension(filename, ".svg"), Renderer.RenderSvg(512, voxelData));
                }
            }
        }
    }
}
