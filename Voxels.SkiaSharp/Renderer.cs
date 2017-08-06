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
                Render(size, voxelData, canvas, ao: true);
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
                        Render(size, voxelData, canvas, ao:false);
                    }
                }
            }
            return ms.ToArray();
        }

        static XYZ[] TopCross = new[] {
            XYZ.OneY,
            XYZ.OneX,
            -XYZ.OneY,
            -XYZ.OneX,
        };
        static XYZ[] LeftCross = new[] {
            XYZ.OneY,
            XYZ.OneZ,
            -XYZ.OneY,
            -XYZ.OneZ,
        };
        static XYZ[] RightCross = new[] {
            XYZ.OneZ,
            XYZ.OneX,
            -XYZ.OneZ,
            -XYZ.OneX,
        };
        static XYZ TopUp = XYZ.OneZ;
        static XYZ RightUp = -XYZ.OneY;
        static XYZ LeftUp = -XYZ.OneX;

        static void Render(int size, VoxelData voxelData, SKCanvas canvas, bool ao) {
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
                            RenderQuad(voxelData, i, canvas, matrix, ao, color*0.8f, LeftCross, LeftUp);
                            RenderQuad(voxelData, i, canvas, matrix, ao, color * 0.9f, RightCross, RightUp);
                            RenderQuad(voxelData, i, canvas, matrix, ao, color, TopCross, TopUp);
                        }
                    }
                }
            }
        }

        static void RenderQuad(VoxelData voxelData, XYZ p, SKCanvas canvas, SKMatrix44 matrix, bool ao, Color color, XYZ[] cross, XYZ up) {
            // Only render quad if face it isn't hidden by voxel above it
            if (voxelData[p + up].colorIndex == 0) {
                // Map voxel coordinates to projected 3D space
                
                var vertices = Enumerable.Range(0, 4)
                    .Select(i => cross[i]+cross[(i+1)%4]+up)
                    .Select(c => c + XYZ.One)
                    .Select(c => matrix.MapScalars(p.X + c.X * .5f, p.Z + c.Z * .5f, -p.Y - c.Y * .5f, 1f))
                    .Select(v => new SKPoint(v[0], v[1]))
                    .ToArray();

                if (ao) {
                    // Create face quad path
                    var col = new SKColor(color.R, color.G, color.B, color.A);
                    var a00 = AmbientOcclusion.CalculateAO(voxelData, p, cross[0], cross[1], up);
                    var a01 = AmbientOcclusion.CalculateAO(voxelData, p, cross[1], cross[2], up);
                    var a11 = AmbientOcclusion.CalculateAO(voxelData, p, cross[2], cross[3], up);
                    var a10 = AmbientOcclusion.CalculateAO(voxelData, p, cross[3], cross[0], up);
                    var colors = new SKColor[] {
                        AOToColor(color, a00),
                        AOToColor(color, a01),
                        AOToColor(color, a11),
                        AOToColor(color, a10),
                    };

                    ushort[] indices;
                    if (a00 + a11 > a01 + a10) {
                        indices = new ushort[] { 0,1,2, 2,3,0, };
                    }
                    else {
                        indices = new ushort[] { 1,2,3, 3,0,1, };
                    }

                    // Calculate Ambient Occlusion
                    using (var fill = new SKPaint {
                        IsAntialias = false,
                        Style = SKPaintStyle.Fill,
                    }) {
                        canvas.DrawVertices(SKVertexMode.Triangles, vertices, null, colors, indices, fill);
                    }
                }
                else {
                    // Create face quad path
                    using (var face = new SKPath()) {
                        face.AddPoly(vertices, close: true);

                        // Calculate Ambient Occlusion
                        using (var fill = new SKPaint {
                            IsAntialias = false,
                            Style = SKPaintStyle.Fill,
                            Color = new SKColor(color.R, color.G, color.B, color.A),
                        }) {
                            canvas.DrawPath(face, fill);
                        }
                    }
                }
            }
        }

        static SKColor AOToColor(Color color, int ao) {
            var c = new SKColor(color.R, color.G, color.B, color.A);
            float h, s, v;
            c.ToHsv(out h, out s, out v);

            float r = 0;
            switch (ao) {
            case 0: r = 0.5f; break;
            case 1: r = 0.75f; break;
            case 2: r = 0.8f; break;
            case 3: r = 1f; break;
            }
            return SKColor.FromHsv(h, s, v * r, c.Alpha);
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
