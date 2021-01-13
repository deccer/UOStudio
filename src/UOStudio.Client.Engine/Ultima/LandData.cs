namespace UOStudio.Client.Engine.Ultima
{
    public struct LandData
    {
        public LandData(string name, TileFlag flags)
        {
            Name = name;
            Flags = flags;
        }

        public string Name { get; set; }

        public TileFlag Flags { get; set; }
    }
}
