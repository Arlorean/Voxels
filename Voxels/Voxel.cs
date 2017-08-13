namespace Voxels {
    public struct Voxel {
        public byte colorIndex;

        public static Voxel Empty;

        public override string ToString() {
            return colorIndex.ToString();
        }
    }
}
