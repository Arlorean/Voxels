using SharpShell.Attributes;
using SharpShell.SharpThumbnailHandler;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Voxels.SkiaSharp;

namespace Voxels.ShellExtensions {
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.FileExtension, ".qb")]
    public class ThumbnailHandlerQb : SharpThumbnailHandler {
        protected override Bitmap GetThumbnailImage(uint width) {
            var size = (int)width;
            var voxelData = QbFile.ReadAndFlatten(SelectedItemStream);
            if (voxelData == null) {
                return null;
            }

            var renderSettings = new RenderSettings() { Size = size };
            var bitmapBytes = Renderer.RenderBitmap(voxelData, renderSettings);

            // Convert Skia bytes to GDI Bitmap
            var format = PixelFormat.Format32bppArgb;
            var bitmap = new Bitmap(size, size, format);
            var bitmapData = bitmap.LockBits(new Rectangle(0,0,size,size), ImageLockMode.WriteOnly, format);
            Marshal.Copy(bitmapBytes, 0, bitmapData.Scan0, bitmapBytes.Length);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        static ThumbnailHandlerQb() {
            NativeLibrary.Initialize();
        }
    }
}
