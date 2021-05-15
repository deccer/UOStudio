using System.IO;
using System.IO.Compression;

namespace UOStudio.Common.Core.HashStorage
{
    public sealed class GZipContentEncoding : IContentEncoding
    {
        /// <inheritdoc />
        public string Name => "gzip";

        /// <summary>
        /// Get and set the compression level to use when writing content. Default is <see cref="System.IO.Compression.CompressionLevel.Optimal"/>.
        /// </summary>
        public CompressionLevel CompressionLevel { get; set; }

        /// <summary>
        /// Initialises a new GZip-based content encoding.
        /// </summary>
        public GZipContentEncoding()
        {
            CompressionLevel = CompressionLevel.Optimal;
        }

        /// <inheritdoc />
        public Stream Encode(Stream stream)
        {
            return new GZipStream(stream, CompressionLevel, leaveOpen: true);
        }

        /// <inheritdoc />
        public Stream Decode(Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Decompress, leaveOpen: true);
        }
    }
}
