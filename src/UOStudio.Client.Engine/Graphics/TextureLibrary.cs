using Ardalis.GuardClauses;
using Serilog;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace UOStudio.Client.Engine.Graphics
{
    internal sealed class TextureLibrary : ITextureLibrary
    {
        private readonly IGraphicsDevice _graphicsDevice;
        private readonly ILogger _logger;
        private readonly IDictionary<string, string> _textureFilePaths;

        private readonly IDictionary<(int Width, int Height), IList<Image>> _imagesPerResolution;
        private readonly IDictionary<(int Width, int Height), int> _arrayIndexPerResolution;
        private readonly IDictionary<string, (int Width, int Height)> _resolutionsPerFilePath;
        private readonly IDictionary<string, int> _textureIndexPerTextureName;

        public TextureLibrary(
            ILogger logger,
            IGraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _logger = logger.ForContext<TextureLibrary>();

            _textureFilePaths = new Dictionary<string, string>(256);

            _imagesPerResolution = new Dictionary<(int Width, int Height), IList<Image>>(256);
            _resolutionsPerFilePath = new Dictionary<string, (int Width, int Height)>(256);
            _arrayIndexPerResolution = new Dictionary<(int Width, int Height), int>(256);
            _textureIndexPerTextureName = new Dictionary<string, int>(256);
        }

        public void Dispose()
        {
            // TODO(deccer): clean up images
        }

        public void AddTextureFromFile(string name, string filePath)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));
            Guard.Against.NullOrEmpty(filePath, nameof(filePath));

            if (_textureFilePaths.ContainsKey(name))
            {
                return;
            }
            
            _textureFilePaths.Add(name, filePath);
        }

        public void LoadResources(
            out IEnumerable<ITextureArray> textureArrays,
            out IDictionary<string, (int ArrayIndex, int TextureIndex)> indicesPerTextureName)
        {
            indicesPerTextureName = new Dictionary<string, (int ArrayIndex, int TextureIndex)>(256);
            foreach (var texturePath in _textureFilePaths)
            {
                var name = texturePath.Key;
                var filePath = texturePath.Value;
                if (filePath.EndsWith(".dds"))
                {
                    filePath = Path.ChangeExtension(filePath, ".png");
                }

                if (_resolutionsPerFilePath.ContainsKey(name))
                {
                    _logger.Warning("TextureLibrary: Texture {@Name} already exists", name);
                    continue;
                }

                if (!File.Exists(filePath))
                {
                    _logger.Error("TextureLibrary: Unable to load texture {@Name}. Path {@FilePath} does not exist.",
                        name, filePath);
                    continue;
                }

                _logger.Debug("TextureLibrary: Loading Texture {@Name} from {@FilePath}", name, filePath);
                var image = Image
                    .Load(filePath)
                    .CloneAs<Rgba32>();
                image.Mutate(ctx => ctx.Flip(FlipMode.Vertical));
                var imageResolution = (image.Width, image.Height);
                if (_imagesPerResolution.TryGetValue(imageResolution, out var imagesPerResolution))
                {
                    imagesPerResolution.Add(image);
                    _textureIndexPerTextureName.Add(name, imagesPerResolution.Count - 1);
                }
                else
                {
                    imagesPerResolution = new List<Image>(128);
                    imagesPerResolution.Add(image);
                    _imagesPerResolution.Add(imageResolution, imagesPerResolution);
                    _textureIndexPerTextureName.Add(name, imagesPerResolution.Count - 1);
                }

                if (_arrayIndexPerResolution.ContainsKey(imageResolution))
                {
                    _arrayIndexPerResolution[imageResolution] = _imagesPerResolution.Count - 1;
                }
                else
                {
                    _arrayIndexPerResolution.Add(imageResolution, _imagesPerResolution.Count - 1);
                }

                if (_resolutionsPerFilePath.ContainsKey(filePath))
                {
                    _resolutionsPerFilePath[filePath] = imageResolution;
                }
                else
                {
                    _resolutionsPerFilePath.Add(filePath, imageResolution);
                }

                indicesPerTextureName.Add(filePath, (ArrayIndexFromResolution(imageResolution), TextureIndexFromName(name)));
            }

            textureArrays = _imagesPerResolution
                .Select(imagePerResolution =>
                    _graphicsDevice.CreateTextureArrayFromImages(imagePerResolution.Value, TextureFormat.Rgba8));
        }

        private int ArrayIndexFromResolution((int Width, int Height) imageResolution)
        {
            return _arrayIndexPerResolution.TryGetValue(imageResolution, out var arrayIndex)
                ? arrayIndex
                : -1;
        }

        private int TextureIndexFromName(string filePath)
        {
            return _textureIndexPerTextureName.TryGetValue(filePath, out var textureIndex)
                ? textureIndex
                : -1;
        }
    }
}