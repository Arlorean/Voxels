using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Voxels.SkiaSharp {
    public class Renderer {
        static void RenderIntoBitmap(int size, VoxelData voxelData, SKBitmap bitmap) {
            using (var canvas = new SKCanvas(bitmap)) {
                bitmap.Erase(SKColors.Transparent);
                Render(size, voxelData, canvas);
            }
        }

        public static byte[] RenderBitmap(int size, VoxelData voxelData) {
            using (var bitmap = new SKBitmap(size, size, false)) {
                RenderIntoBitmap(size, voxelData, bitmap);
                return bitmap.Bytes;
            }
        }

        public static byte[] RenderPng(int size, VoxelData voxelData) {
            using (var bitmap = new SKBitmap(size, size, false)) {
                RenderIntoBitmap(size, voxelData, bitmap);

                using (var image = SKImage.FromBitmap(bitmap)) {
                    using (var data = image.Encode()) {
                        var ms = new MemoryStream();
                        data.SaveTo(ms);
                        return ms.GetBuffer();
                    }
                }
            }
        }

        public static byte[] RenderSvg(int size, VoxelData voxelData) {
            var ms = new MemoryStream();
            using (var skStream = new SKManagedWStream(ms)) {
                using (var writer = new SKXmlStreamWriter(skStream)) {
                    using (var canvas = SKSvgCanvas.Create(SKRect.Create(0, 0, size, size), writer)) {
                        Render(size, voxelData, canvas);
                    }
                }
            }
            return ms.ToArray();
        }

        static XYZ[] TopCorners = new[] {
            new XYZ(-1, -1, +1),
            new XYZ(-1, +1, +1),
            new XYZ(+1, +1, +1),
            new XYZ(+1, -1, +1),
        };
        static XYZ[] LeftCorners = new[] {
            new XYZ(-1, -1, -1),
            new XYZ(-1, +1, -1),
            new XYZ(-1, +1, +1),
            new XYZ(-1, -1, +1),
        };
        static XYZ[] RightCorners = new[] {
            new XYZ(-1, -1, -1),
            new XYZ(-1, -1, +1),
            new XYZ(+1, -1, +1),
            new XYZ(+1, -1, -1),
        };

        static void Render(int size, VoxelData voxelData, SKCanvas canvas) {
            var r = 1.61803398875f;
            var d = size / (r*voxelData.size.MaxDimension);
            var tran = SKMatrix44.CreateTranslate(size / 2, size/2,0);
            var rotx = SKMatrix44.CreateRotationDegrees(1, 0, 0, -26f);
            var roty = SKMatrix44.CreateRotationDegrees(0, 1, 0, 45);
            var matrix = SKMatrix44.CreateIdentity();
            matrix.PreConcat(tran);
            matrix.PreConcat(rotx);
            matrix.PreConcat(roty);
            matrix.PreScale(d, -d, d);
            matrix.PreTranslate(-voxelData.size.X*0.5f, -voxelData.size.Z*0.5f, voxelData.size.Y * 0.5f);

            for (var y = voxelData.size.Y-1; y >=0; --y) {
                for (var x = voxelData.size.X-1; x >= 0; --x) {
                    for (var z = 0; z < voxelData.size.Z; ++z) {
                        var i = new XYZ(x, y, z);

                        var color = voxelData.ColorOf(i);
                        if (color.A > 0) {
                            RenderQuad(voxelData, i, canvas, matrix, color*0.8f, LeftCorners);
                            RenderQuad(voxelData, i, canvas, matrix, color*0.9f, RightCorners);
                            RenderQuad(voxelData, i, canvas, matrix, color, TopCorners);
                        }
                    }
                }
            }
        }

        static void RenderQuad(VoxelData voxelData, XYZ i, SKCanvas canvas, SKMatrix44 matrix, Color color, XYZ[] corners) {
            var up = corners.Aggregate((a,b) => a+b)/4;

            // Only render quad if face it isn't hidden by voxel above it
            if (voxelData[i + up].colorIndex == 0) {
                // Map voxel coordinates to projected 3D space
                var p = corners.Select(c => c+XYZ.One)
                               .Select(c => matrix.MapScalars(i.X + c.X*.5f, i.Z + c.Z*.5f, -i.Y - c.Y*.5f, 1f))
                               .Select(v => new SKPoint(v[0], v[1]))
                               .ToArray();

                // Create face quad path
                using (var face = new SKPath()) {
                    face.AddPoly(new[] { p[0], p[1], p[2], p[3] }, close: true);

                    // Calculate Ambient Occlusion
                    using (var fill = new SKPaint {
                        IsAntialias = false,
                        Style = SKPaintStyle.Fill,
                        //Shader = SKShader.CreateLinearGradient(p[0], p[2],
                        //    new[] { SKColors.White, SKColors.White },
                        //    null,
                        //SKShaderTileMode.Clamp),
                        Color = new SKColor(color.R, color.G, color.B, color.A),
                    }) {
                        canvas.DrawPath(face, fill);
                    }
                }
            }
        }


        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        extern static IntPtr LoadLibrary(string dllPath);

        static Renderer() {
            var dir = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var arch = IntPtr.Size == 8 ? "x64" : "x86";
            var path = Path.Combine(dir, arch, "libSkiaSharp.dll");
            LoadLibrary(path);
        }

    }
}
