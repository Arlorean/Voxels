using SkiaSharp;
using System.IO;
using System.Linq;

namespace Voxels.SkiaSharp {
    public class Renderer {
        static void RenderIntoBitmap(int size, VoxelData voxelData, SKBitmap bitmap) {
            using (var canvas = new SKCanvas(bitmap)) {
                bitmap.Erase(SKColors.Transparent);
                RenderTriangles(voxelData, size, canvas, new MeshSettings {
                    AmbientOcclusion = true,
                    FrontFacesOnly = true,
                    FakeLighting = true,
                    MeshType = MeshType.Triangles,
                });
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
                        return ms.ToArray();
                    }
                }
            }
        }

        public static byte[] RenderSvg(int size, VoxelData voxelData) {
            var ms = new MemoryStream();
            using (var skStream = new SKManagedWStream(ms)) {
                using (var writer = new SKXmlStreamWriter(skStream)) {
                    using (var canvas = SKSvgCanvas.Create(SKRect.Create(0, 0, size, size), writer)) {
                        RenderQuads(voxelData, size, canvas, new MeshSettings {
                            AmbientOcclusion = false,
                            FrontFacesOnly = true,
                            FakeLighting = true,
                            MeshType = MeshType.Quads,
                        });
                    }
                }
            }
            return ms.ToArray();
        }

        static SKMatrix44 GetMatrix(VoxelData voxelData, int size) {
            var r = 1.61803398875f;
            var d = size / (r*voxelData.size.MaxDimension);
            var tran = SKMatrix44.CreateTranslate(size*0.5f, size*0.5f, 0);
            var rotx = SKMatrix44.CreateRotationDegrees(1, 0, 0, -26f);
            var roty = SKMatrix44.CreateRotationDegrees(0, 1, 0, 45);
            var matrix = SKMatrix44.CreateIdentity();
            matrix.PreConcat(tran);
            matrix.PreConcat(rotx);
            matrix.PreConcat(roty);
            matrix.PreScale(d, -d, d);
            matrix.PreTranslate(-voxelData.size.X*0.5f, -voxelData.size.Z*0.5f, voxelData.size.Y*0.5f);
            return matrix;
        }

        static void RenderTriangles(VoxelData voxelData, int size, SKCanvas canvas, MeshSettings settings) {
            var matrix = GetMatrix(voxelData, size);
            settings.MeshType = MeshType.Triangles;
            var triangles = new MeshBuilder(voxelData, settings);

            using (var fill = new SKPaint()) {
                var vertices = triangles.Vertices
                    .Select(v => matrix.MapScalars(v.X, v.Z, -v.Y, 1f))
                    .Select(v => new SKPoint(v[0], v[1]))
                    .ToArray();
                var colors = triangles.Colors.Select(ToSKColor).ToArray();
                var indices = triangles.Faces;
                canvas.DrawVertices(SKVertexMode.Triangles, vertices, null, colors, indices, fill);
            }
        }

        static void RenderQuads(VoxelData voxelData, int size, SKCanvas canvas, MeshSettings settings) {
            var matrix = GetMatrix(voxelData, size);
            settings.MeshType = MeshType.Quads;
            var quads = new MeshBuilder(voxelData, settings);

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
                    using (var fill = new SKPaint() { Color = ToSKColor(color) }) {
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
