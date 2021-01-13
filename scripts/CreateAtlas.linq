<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

class Tile
{
    public int Id;
    public float U0;
    public float V0;
    public float U1;
    public float V1;
    public float U2;
    public float V2;
    public float U3;
    public float V3;
}

void Main()
{
    var tileWidth = 44;
    var tileHeight = 44;
    using var atlas = new Bitmap(6144, 6144);
    var files = Directory.GetFiles(@"C:\Temp\UOF_Land\", "*.png");
    var tiles = new Tile[files.Count()];
    
    var tileCounterX = 0;
    var tileCounterY = 0;
    var maxTilesX = atlas.Width / tileWidth;
    var maxTilesY = atlas.Height / tileHeight;
    
    var atlasTileWidth = 1.0f / atlas.Width * tileWidth;
    var atlasTileHeight = 1.0f / atlas.Height * tileHeight;
    
    using var atlasGraphics = Graphics.FromImage(atlas);
    foreach (var file in files)
    {
        using var tileBitmap = Bitmap.FromFile(file);
        
        atlasGraphics.DrawImageUnscaled(tileBitmap, new Point(tileCounterX * tileWidth, tileCounterY * tileHeight));

        var tileId = int.Parse(Path.GetFileNameWithoutExtension(file));
        tiles[tileId] = new Tile
        {
            Id = tileId,
            U0 = tileCounterX * atlasTileWidth + atlasTileWidth * 0.5f,
            V0 = tileCounterY * atlasTileHeight,
            U1 = tileCounterX * atlasTileWidth + atlasTileWidth,
            V1 = tileCounterY * atlasTileHeight + atlasTileHeight * 0.5f,
            U2 = tileCounterX * atlasTileWidth,
            V2 = tileCounterY * atlasTileHeight + atlasTileHeight * 0.5f,
            U3 = tileCounterX * atlasTileWidth + atlasTileWidth * 0.5f,
            V3 = tileCounterY * atlasTileHeight + atlasTileHeight
        };

        tileCounterX++;
        if (tileCounterX >= maxTilesX)
        {
            tileCounterX = 0;
            tileCounterY++;
        }
    }
    
    atlas.Save(@"C:\Temp\UOFOutput\Atlas.png");
    var json = JsonConvert.SerializeObject(tiles, Newtonsoft.Json.Formatting.Indented);
    File.WriteAllText(@"C:\Temp\UOFOutput\Atlas.json", json);
}

// You can define other methods, fields, classes and namespaces here
