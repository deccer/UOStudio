namespace UOStudio.TextureAtlasGenerator.Contracts
{
    public struct Vertex
    {
        public float U;

        public float V;

        public float W;

        public override string ToString() => $"{U} {V} {W}";
    }
}
