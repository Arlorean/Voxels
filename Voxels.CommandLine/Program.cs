using System.ComponentModel.DataAnnotations;
using System.IO;
using Voxels.SkiaSharp;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Voxels.CommandLine {
    class Program {

        [Option(ShortName = "w", Description = "Size in pixels.")]
        public int Size { get; set; } = 512;

        [Option(ShortName = "y", Description = "The yaw in degrees.")]
        public float Yaw { get; set; } = 45f;

        [Option(ShortName = "x", Description = "The pitch in degrees.")]
        public float Pitch { get; set; } = -26f;

        [Option(Description = "Output a PNG file.")]
        public bool PNG { get; set; }

        [Option(Description = "Output an SVG file.")]
        public bool SVG { get; set; }

        [Option(Description = "Output an animated GIF file.")]
        public bool GIF { get; set; }

        [Option(Description = "Convert PNG file to VOX.")]
        public bool VOX { get; set; }

        [Option(ShortName = "n", Description = "The number of frames for the animated GIF.")]
        public int RotationFrames { get; set; } = 36;

        [Option(ShortName = "d", Description = "The duration for the animated GIF in seconds.")]
        public float RotationDuration { get; set; } = 2f;

        [Option(Description = "Recurse into sub-directories to convert.")]
        public bool Recursive { get; set; }

        [Argument(0, Description = "Filenames or directories to convert."), Required]
        public string[] Filenames { get; set; }

        public static int Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        void OnExecute() {
            // Initialize SkiaSharp
            NativeLibrary.Initialize();

            if (VOX) {
                var colorsUsed = new HashSet<Color>() { Color.Transparent };
                ExtractColors(Filenames, colorsUsed);

                var palette = colorsUsed.ToArray();
                if (palette.Length > 255) {
                    Console.WriteLine($"Warning: More than 255 unique colors exist in the image(s) - truncating palette from {palette.Length} to 256 colors.");
                }
                Array.Resize(ref palette, 256); // Ensure array is exactly 256 colors long

                ConvertFiles(Filenames, palette);
            }
            else {
                // If none of PNG, SVG or GIF is specified, output PNG and SVG (previous default)
                if (!PNG && !SVG && !GIF) {
                    PNG = SVG = true;
                }

                var renderSettings = new RenderSettings() {
                    Size = Size,
                    Pitch = Pitch,
                    Yaw = Yaw,
                };
                RenderFiles(Filenames, renderSettings);
            }
        }

        void RenderFiles(string[] filenames, RenderSettings renderSettings) {
            foreach (var filename in filenames) {
                // Convert all files in directories
                if (Directory.Exists(filename)) {
                    var directoryFilenames = Directory.GetFiles(filename);
                    RenderFiles(directoryFilenames, renderSettings);
                    if (Recursive) {
                        var directoryNames = Directory.GetDirectories(filename);
                        RenderFiles(directoryNames, renderSettings);
                    }
                }
                else {
                    var voxelData = VoxelImport.Import(filename);
                    if (voxelData != null) {
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

        void ExtractColors(string[] filenames, HashSet<Color> colorsUsed) {
            foreach (var filename in filenames) {
                // Convert all files in directories
                if (Directory.Exists(filename)) {
                    var directoryFilenames = Directory.GetFiles(filename);
                    ExtractColors(directoryFilenames, colorsUsed);
                }
                else {
                    ImageToVoxel.ExtractColors(filename, colorsUsed);
                }
            }
        }

        void ConvertFiles(string[] filenames, Color[] pallete) {
            foreach (var filename in filenames) {
                // Convert all files in directories
                if (Directory.Exists(filename)) {
                    var directoryFilenames = Directory.GetFiles(filename);
                    ConvertFiles(directoryFilenames, pallete);
                }
                else {
                    var voxelData = ImageToVoxel.Import(filename, pallete);
                    if (voxelData != null) {
                        using (var stream = File.Create(Path.ChangeExtension(filename, ".vox"))) {
                            var magicaVoxel = new MagicaVoxel(voxelData);
                            magicaVoxel.Write(stream);
                        }
                    }
                }
            }
        }
    }
}
