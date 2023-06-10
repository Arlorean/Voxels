using NGif.Components;
using SkiaSharp;
using System;
using System.IO;
using Voxels.SkiaSharp;

namespace Voxels.CommandLine {
    internal static class Animation {
        public static byte[] RenderGif(RenderSettings renderSettings, int frames, float duration, int cameraOrbits, BoundsXYZ worldBounds, Func<int, VoxelData> flattenFrame) {
            var delay = (int)(duration / frames * 1000);

            var encoder = new AnimatedGifEncoder();
            var memory = new MemoryStream();
            encoder.Start(memory);
            encoder.SetDelay(delay);
            //-1:no repeat,0:always repeat
            encoder.SetRepeat(0);
            encoder.SetTransparent(SKColors.Black);

            //var worldBounds = magicaVoxel.GetWorldAABB(0, frames - 1);

            var startAngle = renderSettings.Yaw;
            var stepAngle = (360*cameraOrbits) / frames;
            for (var i=0; i < frames; i++) {
                renderSettings.Yaw = startAngle + stepAngle * i;

                //var voxelData = magicaVoxel.Flatten(worldBounds, i);
                var voxelData = flattenFrame(i);
                var bytes = Renderer.RenderPng(voxelData, renderSettings);
                var image = SKImage.FromBitmap(SKBitmap.Decode(bytes));
                encoder.AddFrame(image);
            }
            renderSettings.Yaw = startAngle;

            encoder.Finish();

            return memory.ToArray();
        }
    }
}
