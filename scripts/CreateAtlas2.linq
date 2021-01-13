<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

class Tile
{
    public int Id;
    public int AtlasId;
    public int AtlasPageIndex;
    public float U0;
    public float V0;
    public float U1;
    public float V1;
    public float U2;
    public float V2;
    public float U3;
    public float V3;
}

public enum Orientation
{
    Square,
    Landscape,
    Portrait
}

struct BitmapInfo
{
    public BitmapInfo(string fileName, Image image)
    {
        TileId = int.Parse(Path.GetFileNameWithoutExtension(fileName));
        Width = image.Width;
        Height = image.Height;
        Circumference = 2 * (Width + Height);
        Orientation = Height > Width ?
                       Orientation.Portrait :
                        Height == Width ?
                        Orientation.Square : Orientation.Landscape;
    }
    
    public int TileId;
    public int Circumference;
    public Orientation Orientation;
    public int Width;
    public int Height;
}

void NormalizeUoFiddlerOutput(string directory)
{
    var fileNames = Directory.GetFiles(directory, "*.png", SearchOption.TopDirectoryOnly);
    foreach (var fileName in fileNames)
    {
        var splitFileName = Path.GetFileNameWithoutExtension(fileName)
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var staticId = int.Parse(splitFileName[1].Substring(2), NumberStyles.AllowHexSpecifier | NumberStyles.HexNumber);

        var filePath = Path.Combine(directory, $"{staticId}.png");
        File.Move(fileName, filePath);
    }
}

void CreateAtlas(BitmapInfo[] bitmapInfos, Orientation orientation, string inputDirectory, string outputDirectory)
{
    var sw = Stopwatch.StartNew();
    var sortedInfos = bitmapInfos.Where(bi => bi.Orientation == orientation).OrderByDescending(bi => bi.Circumference).ToArray();
    var atlasPages = new List<Bitmap>(16);
    var currentSpriteY = 0;
    var currentSpriteX = 0;
    var atlasPageIndex = 0;
    Graphics currentGraphics = null;
    Stack<int> firstColumn = new Stack<int>();
    var surfaceArea = 0.0f;
    for (var i = 0; i < sortedInfos.Length; i++)
    {
        if (currentSpriteX == 0 && currentSpriteY == 0)
        {
            if (atlasPages.Count > 0)
            {
                currentGraphics?.Dispose();
                atlasPages[atlasPageIndex].Save(Path.Combine(outputDirectory, $"{orientation}{atlasPageIndex}.png"));
                atlasPageIndex++;
            }
            
            if (orientation == Orientation.Square)
            {
                atlasPages.Add( new Bitmap(2048, 2048));
            }
            else if (orientation == Orientation.Landscape)
            {
                atlasPages.Add( new Bitmap(4096, 2048));
            }
            else
            {
                atlasPages.Add( new Bitmap(4096, 2048));                
            }
        }

        var spriteInfo = sortedInfos[i];
        if (currentSpriteX == 0)
        {
            firstColumn.Push(i);
        }
        using var sprite = Bitmap.FromFile(Path.Combine(inputDirectory, $"{spriteInfo.TileId}.png"));

        currentGraphics.DrawImage(sprite, currentSpriteX, currentSpriteY);
        
        surfaceArea += sprite.Width * sprite.Height;

        currentSpriteX += spriteInfo.Width;
        if (currentSpriteX + spriteInfo.Width >= atlasPages[atlasPageIndex].Width)
        {
            currentSpriteX = 0;
            currentSpriteY += sortedInfos[firstColumn.Pop()].Height;
        }
        if (currentSpriteY + spriteInfo.Height >= atlasPages[atlasPageIndex].Height)
        {
            currentSpriteX = 0;
            currentSpriteY = 0;
        }
    }
    atlasPages[atlasPageIndex].Save(Path.Combine(outputDirectory, $"{orientation}{atlasPageIndex}.png"));
    sw.Stop();

    var packingEfficiency = (atlasPageIndex * (atlasPages[atlasPageIndex].Width * atlasPages[atlasPageIndex].Height)) / surfaceArea * 100.0f;
    Debug.WriteLine($"Processing {sortedInfos.Length} {orientation} Sprites Done - {atlasPageIndex + 1} pages - took {sw.Elapsed.TotalSeconds}s - PackingEff: {packingEfficiency:F2}%");
    
    foreach (var page in atlasPages)
    {
        page.Dispose();
    }
    atlasPages.Clear();
}

void Main()
{

    BitmapInfo[] bitmapInfos;
    const string bitmapInfoFileName = @"C:\Temp\bitmapinfos.json";
    const string inputDirectory = @"C:\Temp\UOF_Statics\";
    const string outputDirectory = @"C:\Temp\UOFOutput";
    
    NormalizeUoFiddlerOutput(inputDirectory); // run only once
    if (!File.Exists(bitmapInfoFileName))
    {
        var fileNames = Directory.GetFiles(inputDirectory, "*.png", SearchOption.TopDirectoryOnly);
        var fileCount = fileNames.Count();
        bitmapInfos = new BitmapInfo[fileCount];
        for (var i = 0; i < fileCount; i++)
        {
            var fileName = fileNames[i];
            using var bitmap = Bitmap.FromFile(fileName);

            bitmapInfos[i] = new BitmapInfo(fileName, bitmap);
        }

        var bitmapInfosJson = JsonConvert.SerializeObject(bitmapInfos, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(bitmapInfoFileName, bitmapInfosJson);
    }
    else
    {
        var bitmapInfosJson = File.ReadAllText(bitmapInfoFileName);
        bitmapInfos = JsonConvert.DeserializeObject<BitmapInfo[]>(bitmapInfosJson);
    }

    CreateAtlas(bitmapInfos, Orientation.Portrait, inputDirectory, outputDirectory);
    CreateAtlas(bitmapInfos, Orientation.Landscape, inputDirectory, outputDirectory);
    CreateAtlas(bitmapInfos, Orientation.Square, inputDirectory, outputDirectory);
}