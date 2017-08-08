using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Voxels.SkiaSharp {
    public static class NativeLibrary {

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        extern static IntPtr LoadLibrary(string dllPath);

        public static void Initialize() {
            var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var arch = IntPtr.Size == 8 ? "x64" : "x86";
            var path = Path.Combine(dir, arch, "libSkiaSharp.dll");
            if (LoadLibrary(path) == IntPtr.Zero) {
                Console.Error.WriteLine("Cannot load library: " + path);
            }
        }
    }
}
