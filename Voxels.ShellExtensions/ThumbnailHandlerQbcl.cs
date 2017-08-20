using System.Drawing;
using SharpShell.Attributes;
using SharpShell.SharpThumbnailHandler;
using System.Runtime.InteropServices;
using Voxels.SkiaSharp;
using System.Drawing.Imaging;
using System.IO;
using System;
using System.Reflection;

namespace Voxels.ShellExtensions {
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.FileExtension, ".qbcl")]
    public class ThumbnailHandlerQbcl : SharpThumbnailHandler {
        protected override Bitmap GetThumbnailImage(uint width) {
            var thumb = QbclFile.Read(SelectedItemStream);
            var format = PixelFormat.Format32bppArgb;
            var bitmap = new Bitmap(thumb.Width, thumb.Height, format);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, thumb.Width, thumb.Height), ImageLockMode.WriteOnly, format);
            Marshal.Copy(thumb.Bytes, 0, bitmapData.Scan0, thumb.Bytes.Length);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        static ThumbnailHandlerQbcl() {
            NativeLibrary.Initialize();
        }
    }
}
