namespace UOStudio.TextureAtlasGenerator.Contracts
{
    public struct Uvws
    {
        public void SetW(int atlasPageCount)
        {
            V1.W = V1.W / atlasPageCount;
            V2.W = V2.W / atlasPageCount;
            V3.W = V3.W / atlasPageCount;
            V4.W = V4.W / atlasPageCount;
        }

        public Vertex V1;

        public Vertex V2;

        public Vertex V3;

        public Vertex V4;
    }
}
