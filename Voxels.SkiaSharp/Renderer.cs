using SkiaSharp;
using System;
using System.IO;
using System.Linq;

namespace Voxels.SkiaSharp {
    public class RenderSettings {
        public int Size = 512;
        public float Pitch = -26f;
        public float Yaw = 45f;
    }

    public static class Renderer {
        static void RenderIntoBitmap(VoxelData voxelData, SKBitmap bitmap, RenderSettings renderSettings) {
            using (var canvas = new SKCanvas(bitmap)) {
                bitmap.Erase(SKColors.Transparent);
                RenderTriangles(voxelData, canvas, new MeshSettings {
                    Yaw = renderSettings.Yaw,
                    Pitch = renderSettings.Pitch,
                    FakeLighting = true,
                    FloorShadow = true,
                    MeshType = MeshType.Triangles,
                }, renderSettings);
            }
        }

        public static byte[] RenderBitmap(VoxelData voxelData, RenderSettings renderSettings) {
            var size = renderSettings.Size;
            using (var bitmap = new SKBitmap(size, size, false)) {
                RenderIntoBitmap(voxelData, bitmap, renderSettings);
                return bitmap.Bytes;
            }
        }

        public static byte[] RenderPng(VoxelData voxelData, RenderSettings renderSettings) {
            var size = renderSettings.Size;
            using (var bitmap = new SKBitmap(size, size, false)) {
                RenderIntoBitmap(voxelData, bitmap, renderSettings);

                using (var image = SKImage.FromBitmap(bitmap)) {
                    using (var data = image.Encode()) {
                        var ms = new MemoryStream();
                        data.SaveTo(ms);
                        return ms.ToArray();
                    }
                }
            }
        }

        public static byte[] RenderSvg(VoxelData voxelData, RenderSettings renderSettings) {
            var size = renderSettings.Size;
            var ms = new MemoryStream();
            using (var skStream = new SKManagedWStream(ms)) {
                using (var writer = new SKXmlStreamWriter(skStream)) {
                    using (var canvas = SKSvgCanvas.Create(SKRect.Create(0, 0, size, size), writer)) {
                        RenderQuads(voxelData, size, canvas, new MeshSettings {
                            Yaw = renderSettings.Yaw,
                            Pitch = renderSettings.Pitch,
                            FakeLighting = true,
                            MeshType = MeshType.Quads,
                        }, renderSettings);
                    }
                }
            }
            return ms.ToArray();
        }

        static SKMatrix44 GetMatrix(VoxelData voxelData, RenderSettings renderSettings) {
            var s = renderSettings.Size;
            var r = 1.61803398875f;
            var d = s / (r*voxelData.Size.MaxDimension);
            var tran = SKMatrix44.CreateTranslate(s*0.5f, s*0.5f, 0);
            var rotx = SKMatrix44.CreateRotationDegrees(1, 0, 0, renderSettings.Pitch);
            var roty = SKMatrix44.CreateRotationDegrees(0, 1, 0, renderSettings.Yaw);
            var matrix = SKMatrix44.CreateIdentity();
            matrix.PreConcat(tran);
            matrix.PreConcat(rotx);
            matrix.PreConcat(roty);
            matrix.PreScale(d, -d, d);
            matrix.PreTranslate(-voxelData.Size.X*0.5f, -voxelData.Size.Z*0.5f, voxelData.Size.Y*0.5f);
            return matrix;
        }

        static void RenderTriangles(VoxelData voxelData, SKCanvas canvas, MeshSettings settings, RenderSettings renderSettings) {
            var matrix = GetMatrix(voxelData, renderSettings);
            settings.MeshType = MeshType.Triangles;
            var triangles = new MeshBuilder(voxelData, settings);

            using (var fill = new SKPaint() { IsAntialias = true, FilterQuality = SKFilterQuality.High }) {
                var vertices = triangles.Vertices
                    .Select(v => matrix.MapScalars(v.X, v.Z, -v.Y, 1f))
                    .Select(v => new SKPoint(v[0], v[1]))
                    .ToArray();
                var colors = triangles.Colors;
                var occlusion = triangles.Occlusion;
                var indices = triangles.Faces;

                // Render triangles in batches since SkiaSharp DrawVertices indices are 16 bit which fails for large files
                var batchSize = 3*20000; // up to 20000 triangles per batch
                for (var i = 0; i < indices.Length; i += batchSize) {
                    var batch = Enumerable.Range(i, Math.Min(batchSize, indices.Length - i));
                    var _vertices = batch.Select(j => vertices[indices[j]]).ToArray();
                    var _colors = batch.Select(j => AmbientOcclusion.CombineColorOcclusion(colors[indices[j]], occlusion[indices[j]])).Select(ToSKColor).ToArray();
                    var _indices = batch.Select(j => (ushort)(j-i)).ToArray();
                    canvas.DrawVertices(SKVertexMode.Triangles, _vertices, null, _colors, _indices, fill);
                }
            }
        }

        static void RenderQuads(VoxelData voxelData, int size, SKCanvas canvas, MeshSettings meshSettings, RenderSettings renderSettings) {
            var matrix = GetMatrix(voxelData, renderSettings);
            meshSettings.MeshType = MeshType.Quads;
            var quads = new MeshBuilder(voxelData, meshSettings);

            var vertices = quads.Vertices
                .Select(v => matrix.MapScalars(v.X, v.Z, -v.Y, 1f))
                .Select(v => new SKPoint(v[0], v[1]))
                .ToArray();
            var indices = quads.Faces;
            for (var i=0; i < indices.Length; i += 4) {
                using (var path = new SKPath()) {
                    var quad = Enumerable.Range(0, 4)
                        .Select(n => vertices[indices[i + n]])
                        .ToArray();
                    path.AddPoly(quad, close: true);

                    var color = quads.Colors[quads.Faces[i]]; // Take 1st vertex color for face
                    using (var fill = new SKPaint() {
                        Color = ToSKColor(color),
                        Style=SKPaintStyle.StrokeAndFill,
                        StrokeJoin=SKStrokeJoin.Round,
                    }) {
                        canvas.DrawPath(path, fill);
                    }
                }
            }
        }

        static SKColor ToSKColor(Color c) {
            return new SKColor(c.R, c.G, c.B, c.A);
        }
    }
}
