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

        [Option(Description = "Output a 3D Texture for Unity.", LongName = "3D")]
        public bool Texture3D { get; set; }

        [Option(Description = "The number of frames for the animated GIF.")]
        public int Frames { get; set; } = 30;

        [Option(Description = "The duration for the animated GIF in seconds.")]
        public float Duration { get; set; } = 2f;

        [Option(Description = "The number of camera orbits for the animated GIF (0=none, -1=clockwise.")]
        public int CameraOrbits { get; set; } = 1;

        [Option(Description = "Recurse into sub-directories to convert.")]
        public bool Recursive { get; set; }

        [Option(Description = "Output filename ({0} - path minus extension, {1} - extension).")]
        public string Output { get; set; } = "{0}.{1}";

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
                if (!PNG && !SVG && !GIF && !Texture3D) {
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
                    if (GIF) {
                        using (var stream = File.OpenRead(filename)) {
                            // Support animation frames in MagicaVoxel
                            if (Path.GetExtension(filename).ToLower() == ".vox") {
                                var magicaVoxel = new MagicaVoxel();
                                if (magicaVoxel.Read(stream)) {
                                    var worldBounds = magicaVoxel.GetWorldAABB(0, Frames - 1);
                                    WriteOutput(filename, "gif",
                                        Animation.RenderGif(renderSettings, Frames, Duration, CameraOrbits,
                                            worldBounds, i => magicaVoxel.Flatten(worldBounds, i))
                                    );
                                }
                            }
                            else {
                                var voxelData = VoxelImport.Import(filename);
                                var worldBounds = new BoundsXYZ(voxelData.Size);
                                WriteOutput(filename, "gif",
                                    Animation.RenderGif(renderSettings, Frames, Duration, CameraOrbits,
                                        worldBounds, _ => voxelData)
                                );
                            }
                        }
                    }
                    else {
                        var voxelData = VoxelImport.Import(filename);
                        if (voxelData != null) {
                            if (PNG) {
                                WriteOutput(filename, "png", Renderer.RenderPng(voxelData, renderSettings));
                            }
                            if (SVG) {
                                WriteOutput(filename, "svg", Renderer.RenderSvg(voxelData, renderSettings));
                            }
                            if (Texture3D) {
                                Slicer.MakeSquare(voxelData.Size, out var rows, out var columns);

                                WriteOutput(filename, $"({columns}x{rows}).png", Slicer.RenderTexture3D(voxelData, columns, rows));
                            }
                        }
                    }
                }
            }
        }

        void WriteOutput(string filename, string extension, byte[] bytes) {
            var outputFilename = string.Format(Output, Path.ChangeExtension(filename, null), extension);
            File.WriteAllBytes(outputFilename, bytes);
        }

        void ExtractColors(string[] filenames, HashSet<Color> colorsUsed) {
            foreach (var filename in filenames) {
                // Convert all files in directories
                if (Directory.Exists(filename)) {
                    var directoryFilenames = Directory.GetFiles(filename);
                    ExtractColors(directoryFilenames, colorsUsed);
                    if (Recursive) {
                        var directoryNames = Directory.GetDirectories(filename);
                        ExtractColors(directoryNames, colorsUsed);
                    }
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
                    if (Recursive) {
                        var directoryNames = Directory.GetDirectories(filename);
                        ConvertFiles(directoryNames, pallete);
                    }
                }
                else {
                    var voxelData = ImageToVoxel.Import(filename, pallete);
                    if (voxelData != null) {
                        using (var stream = File.Create(Path.ChangeExtension(filename, ".vox"))) {
                            var magicaVoxel = new MagicaVoxel((VoxelDataBytes)voxelData);
                            magicaVoxel.Write(stream);
                        }
                    }
                }
            }
        }
    }
}
