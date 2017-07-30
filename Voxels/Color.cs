using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Voxels {
    [StructLayout(LayoutKind.Explicit)]
    public struct Color {
        [FieldOffset(0)]
        public uint RGBA;

        [FieldOffset(0)]
        public byte R;
        [FieldOffset(1)]
        public byte G;
        [FieldOffset(2)]
        public byte B;
        [FieldOffset(3)]
        public byte A;

        public Color(byte r, byte g, byte b, byte a) {
            RGBA = 0;
            R = r;
            G = g;
            B = b;
            A = a;
        }
        public Color(uint rgba) {
            R = G = B = A = 0;
            RGBA = rgba;
        }

        public static Color operator * (Color c, float f) {
            return new Color((byte)(c.R * f), (byte)(c.G * f), (byte)(c.B * f), c.A);
        }

        public override string ToString() {
            return RGBA.ToString("X8");
        }
    }
}
