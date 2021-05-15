using System.IO;
using System.IO.Compression;

namespace UOStudio.Common.Core.HashStorage
{
    /// <summary>
    /// Encodes content using deflate compression.
    /// </summary>
    public sealed class DeflateContentEncoding : IContentEncoding
    {
        /// <inheritdoc />
        public string Name => "deflate";

        /// <summary>
        /// Get and set the compression level to use when writing content. Default is <see cref="System.IO.Compression.CompressionLevel.Optimal"/>.
        /// </summary>
        public CompressionLevel CompressionLevel { get; set; }

        /// <summary>
        /// Initialises a new deflate-based content encoding.
        /// </summary>
        public DeflateContentEncoding()
        {
            CompressionLevel = CompressionLevel.Optimal;
        }

        /// <inheritdoc />
        public Stream Encode(Stream stream)
        {
            return new DeflateStream(stream, CompressionLevel, leaveOpen: true);
        }

        /// <inheritdoc />
        public Stream Decode(Stream stream)
        {
            return new DeflateStream(stream, CompressionMode.Decompress, leaveOpen: true);
        }
    }
}
