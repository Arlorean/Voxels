using System;
using System.Runtime.InteropServices;

namespace Voxels {
    [StructLayout(LayoutKind.Explicit)]
    public struct Color : IEquatable<Color> {
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

        public Color(byte r, byte g, byte b) {
            RGBA = 0;
            R = r;
            G = g;
            B = b;
            A = 255;
        }
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
        public Color(Color c, byte a) {
            RGBA = 0;
            R = c.R;
            G = c.G;
            B = c.B;
            A = a;
        }

        public static Color operator * (Color c, float f) {
            return new Color((byte)(c.R * f), (byte)(c.G * f), (byte)(c.B * f), c.A);
        }

        public void ToHSV(out float hue, out float saturation, out float value) {
            var r = R / 255f;
            var g = G / 255f;
            var b = B / 255f;

            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));

            if (max == r) {
                hue = (g - b) / (max - min);
            }
            else if (max == g) {
                hue = 2f + (b - r) / (max - min);
            }
            else {
                hue = 4f + (r - g) / (max - min);
            }

            hue = (float)Math.Round(hue * 60);
            if (hue < 0) {
                hue = hue + 360;
            }
            hue = float.IsNaN(hue) ? 0 : hue;
            saturation = (max == 0) ? 0 : 1 - (min / max);
            value = max;
        }

        public static Color FromHSV(float hue, float saturation, float value) {
            var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            var f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            var v = Convert.ToByte(value);
            var p = Convert.ToByte(value * (1 - saturation));
            var q = Convert.ToByte(value * (1 - f * saturation));
            var t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return new Color(v, t, p);
            else if (hi == 1)
                return new Color(q, v, p);
            else if (hi == 2)
                return new Color(p, v, t);
            else if (hi == 3)
                return new Color(p, q, v);
            else if (hi == 4)
                return new Color(t, p, v);
            else
                return new Color(v, p, q);
        }

        public override string ToString() {
            return RGBA.ToString("X8");
        }

        public override int GetHashCode() {
            return (int) RGBA;
        }

        public override bool Equals(object other) {
            return Equals((Color)other);
        }

        public bool Equals(Color other) {
            return this.RGBA == other.RGBA;
        }

        public static readonly Color Black = new Color(0, 0, 0);
        public static readonly Color White = new Color(255, 255, 255);
        public static readonly Color Red = new Color(255, 0, 0);
        public static readonly Color Green = new Color(0, 255, 0);
        public static readonly Color Blue = new Color(0, 0, 255);
        public static readonly Color Yellow = new Color(255, 255, 0);
        public static readonly Color Cyan = new Color(0, 255, 255);
        public static readonly Color Magenta = new Color(255, 0, 255);
        public static readonly Color Silver = new Color(192, 192, 192);
        public static readonly Color Gray = new Color(128, 128, 128);
        public static readonly Color Maroon = new Color(128, 0, 0);
        public static readonly Color Olive = new Color(128, 128, 0);
        public static readonly Color Tan = new Color(210, 180, 140);
    }
}
