namespace UOStudio.TextureAtlasGenerator.Contracts
{
    public abstract class Tile
    {
        protected Tile(int id, Uvws uvws)
        {
            Id = id;
            Uvws = uvws;
        }

        public int Id;

        public Uvws Uvws;
    }
}
