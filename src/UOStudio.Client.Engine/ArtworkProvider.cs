using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using UOStudio.Client.Engine.IO;

namespace UOStudio.Client.Engine
{
    internal sealed class ArtworkProvider
    {
        private readonly ILogger _logger;
        private bool _isInitialized;
        private FileIndexBase _fileIndex;
        private readonly bool _isUOPFileIndex;
        private readonly string _clientPath;

        public ArtworkProvider(ILogger logger, string ultimaOnlineBasePath, bool isUopFileIndex)
        {
            _logger = logger;
            _isUOPFileIndex = isUopFileIndex;
            _clientPath = ultimaOnlineBasePath;
            Initialize();
        }

        public int Length => _fileIndex.Length;

        private void Initialize()
        {
            _fileIndex = _isUOPFileIndex
                ? CreateFileIndex("artLegacyMUL.uop", 0x10000, false, ".tga")
                : CreateFileIndex("artidx.mul", "art.mul");

            _logger.Debug(_fileIndex.FilesExist.ToString());

            _isInitialized = true;
        }

        private string GetPath(string filename, params object[] args)
        {
            var filePath = Path.Combine(_clientPath, string.Format(filename, args));

            if (!File.Exists(filePath))
            {
                _logger.Error($"{filePath} does not exists.");
            }

            return filePath;
        }

        private FileIndexBase CreateFileIndex(string uopFile, int length, bool hasExtra, string extension)
        {
            uopFile = GetPath(uopFile);

            var fileIndex = new UopFileIndex(uopFile, length, hasExtra, extension);

            if (!fileIndex.FilesExist)
            {
                _logger.Error($"FileIndex was created but {Path.GetFileName(uopFile)} was missing from {_clientPath}");
            }

            fileIndex.Open();

            return fileIndex;
        }

        private FileIndexBase CreateFileIndex(string idxFile, string mulFile)
        {
            idxFile = GetPath(idxFile);
            mulFile = GetPath(mulFile);

            var fileIndex = new MulFileIndex(idxFile, mulFile);

            if (!fileIndex.FilesExist)
            {
                _logger.Error(
                    $"FileIndex was created but 1 or more files do not exist.  Either {Path.GetFileName(idxFile)} or {Path.GetFileName(mulFile)} were missing from {_clientPath}"
                );
            }

            fileIndex.Open();

            return fileIndex;
        }

        public unsafe Texture GetLand(GraphicsDevice graphicsDevice, int index)
        {
            index &= 0x3FFF;

            using var stream = _fileIndex.Seek(index, out _, out _);
            if (stream == null)
            {
                return null;
            }
            using var binaryReader = new BinaryReader(stream);
            var texture = new Texture2D(graphicsDevice, 44, 44, false, SurfaceFormat.Bgra5551);
            var buffer = new ushort[44 * 44];

            var xOffset = 21;
            var xRun = 2;

            fixed (ushort* start = buffer)
            {
                var ptr = start;
                var delta = texture.Width;

                for (var y = 0; y < 22; ++y, --xOffset, xRun += 2, ptr += delta)
                {
                    var cur = ptr + xOffset;
                    var end = cur + xRun;

                    while (cur < end)
                    {
                        *cur++ = (ushort)(binaryReader.ReadUInt16() | 0x8000);
                    }
                }

                xOffset = 0;
                xRun = 44;

                for (var y = 0; y < 22; ++y, ++xOffset, xRun -= 2, ptr += delta)
                {
                    var cur = ptr + xOffset;
                    var end = cur + xRun;

                    while (cur < end)
                    {
                        *cur++ = (ushort)(binaryReader.ReadUInt16() | 0x8000);
                    }
                }
            }

            texture.SetData(buffer);

            return texture;
        }

        public Task<Texture> GetLandAsync(GraphicsDevice graphicsDevice, int index) => Task.FromResult(GetLand(graphicsDevice, index));

        public unsafe Texture2D GetStatic(GraphicsDevice graphicsDevice, int index)
        {
            index += 0x4000;
            index &= 0xFFFF;

            using var stream = _fileIndex.Seek(index, out _, out _);
            using var binaryReader = new BinaryReader(stream);
            binaryReader.ReadInt32(); // Unknown

            int width = binaryReader.ReadInt16();
            int height = binaryReader.ReadInt16();

            if (width <= 0 || height <= 0)
            {
                return null;
            }

            var lookups = new int[height];
            var lookupStart = (int)binaryReader.BaseStream.Position + (height * 2);

            for (var i = 0; i < height; ++i)
            {
                lookups[i] = lookupStart + binaryReader.ReadUInt16() * 2;
            }

            var texture = new Texture2D(graphicsDevice, width, height, false, SurfaceFormat.Bgra5551);
            var bitmapData = new ushort[width * height];

            fixed (ushort* start = bitmapData)
            {
                var ptr = start;
                var delta = texture.Width;

                for (var y = 0; y < height; ++y, ptr += delta)
                {
                    binaryReader.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                    var cur = ptr;

                    int xOffset, xRun;

                    while (((xOffset = binaryReader.ReadUInt16()) + (xRun = binaryReader.ReadUInt16())) != 0)
                    {
                        cur += xOffset;
                        var end = cur + xRun;

                        while (cur < end)
                        {
                            *cur++ = (ushort)(binaryReader.ReadUInt16() ^ 0x8000);
                        }
                    }
                }
            }

            texture.SetData(bitmapData);

            return texture;
        }

        public Task<Texture2D> GetStaticAsync(GraphicsDevice graphicsDevice, int index) => Task.FromResult(GetStatic(graphicsDevice, index));
    }
}
