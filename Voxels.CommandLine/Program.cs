using System.IO;
using Voxels.SkiaSharp;
using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;

namespace Voxels.CommandLine {
    class Program {

        [Option(ShortName = "w", Description = "Size in pixels.")]
        public int Size { get; set; } = 512;

        [Option(ShortName = "y", Description = "The yaw in degrees.")]
        public float Yaw { get; set; } = 135f;

        [Option(ShortName = "x", Description = "The pitch in degrees.")]
        public float Pitch { get; set; } = 26f;

        [Option(Description = "Output a PNG file.")]
        public bool PNG { get; set; }

        [Option(Description = "Output an SVG file.")]
        public bool SVG { get; set; }

        [Option(Description = "Output an animated GIF file.")]
        public bool GIF { get; set; }

        [Option(ShortName = "n", Description = "The number of frames for the animated GIF.")]
        public int RotationFrames { get; set; } = 36;

        [Option(ShortName = "d", Description = "The duration for the animated GIF in seconds.")]
        public float RotationDuration { get; set; } = 2f;

        [Argument(0, Description = "Filenames or directories to convert."), Required]
        public string[] Filenames { get; set; }

        public static int Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        void OnExecute() {
            // If none of PNG, SVG or GIF is specified, output PNG and SVG
            if (!PNG && !SVG && !GIF) {
                PNG = SVG = true;
            }

            // Initialize SkiaSharp
            NativeLibrary.Initialize();

            var renderSettings = new RenderSettings() {
                size = Size,
                rotationX = Math.Abs(Pitch), // Don't allow negative Pitch
                rotationY = Yaw,
            };
            ConvertFiles(Filenames, renderSettings);
        }

        void ConvertFiles(string[] filenames, RenderSettings renderSettings) {
            foreach (var filename in filenames) {
                // Convert all files in directories
                if (Directory.Exists(filename)) {
                    var directoryFilenames = Directory.GetFiles(filename);
                    ConvertFiles(directoryFilenames, renderSettings);
                }
                else {
                    var voxelData = VoxelImport.Import(filename);
                    if (PNG) {
                        File.WriteAllBytes(Path.ChangeExtension(filename, ".png"), Renderer.RenderPng(voxelData, renderSettings));
                    }
                    if (SVG) {
                        File.WriteAllBytes(Path.ChangeExtension(filename, ".svg"), Renderer.RenderSvg(voxelData, renderSettings));
                    }
                    if (GIF) {
                        File.WriteAllBytes(Path.ChangeExtension(filename, ".gif"), Animation.RenderGif(voxelData, renderSettings, RotationFrames, RotationDuration));
                    }
                }
            }
        }
    }
}
