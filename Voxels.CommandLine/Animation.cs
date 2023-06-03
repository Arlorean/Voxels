using NGif.Components;
using SkiaSharp;
using System.IO;
using Voxels.SkiaSharp;

namespace Voxels.CommandLine {
    internal static class Animation {
        public static byte[] RenderGif(VoxelData voxelData, RenderSettings renderSettings, int frames, float duration) {
            var delay = (int)(duration / frames * 1000);

            var encoder = new AnimatedGifEncoder();
            var memory = new MemoryStream();
            encoder.Start(memory);
            encoder.SetDelay(delay);
            //-1:no repeat,0:always repeat
            encoder.SetRepeat(0);
            encoder.SetTransparent(SKColors.Black);

            var startAngle = renderSettings.Yaw;
            var stepAngle = 360 / frames;
            for (var i=0; i < frames; i++) {
                renderSettings.Yaw = startAngle + stepAngle * i;

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
