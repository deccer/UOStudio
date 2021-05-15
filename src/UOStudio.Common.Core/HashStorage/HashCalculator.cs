using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace UOStudio.Common.Core.HashStorage
{
    public static class HashCalculator
    {
        /// <summary>
        /// Compute the hash over the contents of <paramref name="stream" />.
        /// </summary>
        /// <param name="stream">The stream to process.</param>
        /// <returns>The hash of <paramref name="stream"/>'s contents.</returns>
        public static ContentHash Compute(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var hashFunction = SHA1.Create();
            return ContentHash.FromBytes(hashFunction.ComputeHash(stream));
        }

        /// <summary>
        /// Compute the hash over the contents of <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path of a file to process.</param>
        /// <param name="bufferSize">Optional size of the buffer to use when reading chunks from the stream. Defaults to 4096.</param>
        /// <returns>The hash of <paramref name="path"/>'s contents.</returns>
        public static ContentHash Compute(string path, int bufferSize = 4096)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan);
            return Compute(fileStream);
        }

        /// <summary>
        /// Compute the hash over the contents of <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path of a file to process.</param>
        /// <param name="bufferSize">Optional size of the buffer to use when reading chunks from the stream. Defaults to 4096.</param>
        /// <returns>A task that yields the hash of <paramref name="path"/>'s contents.</returns>
        public static async Task<ContentHash> ComputeAsync(string path, int bufferSize = 4096)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            await using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan);
            return await ComputeAsync(fileStream, bufferSize);
        }

        /// <summary>
        /// Compute the hash over the contents of <paramref name="stream" />, reading asynchronously from the stream.
        /// </summary>
        /// <param name="stream">The stream to process.</param>
        /// <param name="bufferSize">Optional size of the buffer to use when reading chunks from the stream. Defaults to 4096.</param>
        /// <returns>A task that yields the hash of <paramref name="stream"/>'s contents.</returns>
        public static async Task<ContentHash> ComputeAsync(Stream stream, int bufferSize = 4096)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var hashFunction = IncrementalHash.CreateHash(HashAlgorithmName.SHA1);
            return ContentHash.FromBytes(await hashFunction.ComputeHashAsync(stream, bufferSize));
        }
    }
}
