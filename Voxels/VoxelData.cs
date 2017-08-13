using System;
using System.Linq;

namespace Voxels {
    /// <summary>
    /// Represents a set of voxels in a fixed size grid.
    /// NOTE: XY is the horizontal plane and Z is the vertical axis.
    /// </summary>
    public class VoxelData {
        public readonly XYZ size;
        readonly Voxel[] voxels;
        readonly Color[] colors;

        public VoxelData(XYZ size, Color[] colors) {
            this.size = size;
            this.voxels = new Voxel[size.Volume];
            this.colors = colors ?? new Color[256];
        }

        public int Count {
            get { return voxels.Count(v => v.colorIndex != 0); }
        }

        public bool IsValid(XYZ p) {
            return (p.X >= 0 && p.X < size.X)
                && (p.Y >= 0 && p.Y < size.Y)
                && (p.Z >= 0 && p.Z < size.Z);
        }

        public Voxel this[XYZ p] {
            get {
                if (IsValid(p)) {
                    return voxels[p.X * (size.Y * size.Z) + p.Y * size.Z + p.Z];
                }
                return Voxel.Empty;
            }
            set {
                if (IsValid(p)) {
                    voxels[p.X * (size.Y * size.Z) + p.Y * size.Z + p.Z] = value;
                }
                else {
                    throw new ArgumentOutOfRangeException("p", p, "point not in voxel data set.");
                }
            }
        }

        public Color[] Colors { get { return colors; } }

        public Color ColorOf(Voxel voxel) {
            return colors[voxel.colorIndex];
        }
        public Color ColorOf(XYZ p) {
            return ColorOf(this[p]);
        }
    }
}
