﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace UOStudio.Common.Core.HashStorage
{
    public sealed class HashedHashedContentStore : IHashedContentStore
    {
        /// <summary>The root path for all content within this store.</summary>
        private readonly string _contentPath;

        /// <summary>The size of the byte array buffer used for read/write operations.</summary>
        private const int BufferSize = 4096;

        /// <summary>The number of characters from the hash to use for the name of the top level subdirectories.</summary>
        private const int HashPrefixLength = 4;

        private static readonly ReaderWriterLockSlim _fileSystemLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// Initialises the store to use <paramref name="contentPath"/> as the root for all content.
        /// </summary>
        /// <remarks>
        /// If <paramref name="contentPath"/> does not exist, it is created.
        /// </remarks>
        /// <param name="contentPath">The root for all content.</param>
        /// <exception cref="T:System.IO.IOException">The directory specified by <paramref name="contentPath"/> is a file.-or-The network name is not known.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="contentPath"/> is a zero-length string, contains only white space, or contains one or more invalid characters.-or-<paramref name="contentPath"/> is prefixed with, or contains, only a colon character (:).</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="contentPath"/> is null.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="T:System.NotSupportedException"><paramref name="contentPath"/> contains a colon character (:) that is not part of a drive label ("C:\").</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public HashedHashedContentStore(string contentPath)
        {
            _contentPath = contentPath ?? throw new ArgumentNullException(nameof(contentPath));

            if (!Directory.Exists(_contentPath))
            {
                Directory.CreateDirectory(_contentPath);
            }
        }

        /// <inheritdoc />
        public async Task<ContentHash> WriteAsync(Stream stream, CancellationToken cancellationToken = new CancellationToken(), IEnumerable<IContentEncoding> encodings = null)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            // Create a new, empty temporary file
            // We will write the source content into this file, whilst computing the content's hash
            // Once we have the hash, we can move this temp file into the correct location
            var tempFile = Path.GetRandomFileName();

            // Create a SHA-1 hash builder
            using var hashBuilder = IncrementalHash.CreateHash(HashAlgorithmName.SHA1);
            // Open the temp file for write
            await using (var fileStream = new FileStream(tempFile,
                FileMode.Open, FileAccess.Write, FileShare.None,
                BufferSize, FileOptions.SequentialScan | FileOptions.Asynchronous))
            {
                // Allocate a buffer, used to process data in chunks
                // We use parallel read/write for increased throughput which requires two buffers
                var buffers = new[] { new byte[BufferSize], new byte[BufferSize] };

                var bufferIndex = 0;
                var writeTask = (Task)null;

                // Loop until the source stream is exhausted
                while (true)
                {
                    // Swap buffers
                    bufferIndex ^= 1;

                    // Start read a chunk of data into the buffer asynchronously
                    var readTask = stream.ReadAsync(buffers[bufferIndex], 0, BufferSize, cancellationToken);

                    if (writeTask != null)
                    {
                        await Task.WhenAll(readTask, writeTask);
                    }

                    var readCount = readTask.Result;

                    // If the stream has ended, break
                    if (readCount == 0)
                    {
                        break;
                    }

                    // Integrate the source data chunk into the hash
                    hashBuilder.AppendData(buffers[bufferIndex], 0, readCount);

                    // Write the source data chunk to the output file
                    writeTask = fileStream.WriteAsync(buffers[bufferIndex], 0, readCount, cancellationToken);
                }
            }

            // Retrieve the computed hash
            var hash = ContentHash.FromBytes(hashBuilder.GetHashAndReset());

            // Determine the location for the content file
            GetPaths(hash, null, out string subPath, out string contentPath);

            // We might need to lock some file system operations
            _fileSystemLock.EnterUpgradeableReadLock();
            try
            {
                // Test whether a file already exists for this hash
                if (File.Exists(contentPath))
                {
                    // This content already exists in the store
                    // Delete the temporary file
                    // NOTE a write lock is not needed here
                    File.Delete(tempFile);
                }
                else
                {
                    // We're about to start changing the file system, so take a write lock
                    _fileSystemLock.EnterWriteLock();
                    try
                    {
                        // Ensure the sub-path exists
                        if (!Directory.Exists(subPath))
                        {
                            Directory.CreateDirectory(subPath);
                        }

                        // Move the temporary file into its correct location
                        File.Move(tempFile, contentPath);

                        // Set the read-only flag on the file
                        File.SetAttributes(contentPath, FileAttributes.ReadOnly);
                    }
                    finally
                    {
                        _fileSystemLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _fileSystemLock.ExitUpgradeableReadLock();
            }

            // Write any encoded forms of the content too
            if (encodings != null)
            {
                foreach (var encoding in encodings)
                {
                    var encodedContentPath = contentPath + "." + encoding.Name;

                    if (File.Exists(encodedContentPath))
                    {
                        continue;
                    }

                    // Create a new temporary file for the encoded content
                    tempFile = Path.GetRandomFileName();

                    await using (var inputStream = new FileStream(contentPath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, FileOptions.SequentialScan | FileOptions.Asynchronous))
                    {
                        await using (var outputStream = new FileStream(tempFile, FileMode.Open, FileAccess.Write, FileShare.None, BufferSize, FileOptions.SequentialScan | FileOptions.Asynchronous))
                        {
                            await using (var encodedOutputStream = encoding.Encode(outputStream))
                            {
                                await inputStream.CopyToAsync(encodedOutputStream, BufferSize, cancellationToken);
                            }
                        }
                    }

                    // Move the temporary file into its correct location
                    File.Move(tempFile, encodedContentPath);

                    // Set the read-only flag on the file
                    File.SetAttributes(encodedContentPath, FileAttributes.ReadOnly);
                }
            }

            // The caller receives the hash, regardless of whether the
            // file previously existed in the store
            return hash;
        }

        /// <inheritdoc />
        public bool Contains(ContentHash contentHash, string encodingName = null)
        {
            return File.Exists(GetContentPath(contentHash, encodingName));
        }

        /// <inheritdoc />
        public bool TryRead(ContentHash contentHash, out Stream stream, ReadOptions options = ReadOptions.None, string encodingName = null)
        {
            var contentPath = GetContentPath(contentHash, encodingName);

            if (!File.Exists(contentPath))
            {
                stream = null;
                return false;
            }

            stream = new FileStream(contentPath,
                FileMode.Open, FileAccess.Read, FileShare.Read,
                BufferSize, (FileOptions)options);

            return true;
        }

        /// <inheritdoc />
        public bool TryGetContentLength(ContentHash contentHash, out long length, string encodingName = null)
        {
            var contentPath = GetContentPath(contentHash, encodingName);

            if (!File.Exists(contentPath))
            {
                length = 0;
                return false;
            }

            length = new FileInfo(contentPath).Length;
            return true;
        }

        /// <inheritdoc />
        public IEnumerable<ContentHash> GetHashes()
        {
            var topLevelRegex = new Regex("^[0-9a-f]{" + HashPrefixLength + "}$", RegexOptions.IgnoreCase);
            var subLevelRegex = new Regex("^[0-9a-f]{" + (ContentHash.StringLength - HashPrefixLength) + "}$", RegexOptions.IgnoreCase);

            var directories = Directory.GetDirectories(_contentPath)
                .Select(p => Path.GetFileName(p))
                .Where(d => topLevelRegex.IsMatch(d));

            return from directory in directories
                let subPath = Path.Combine(_contentPath, directory)
                let files = Directory.GetFiles(subPath).Select(Path.GetFileName).Where(f => subLevelRegex.IsMatch(f))
                from file in files
                select ContentHash.Parse(directory + file);
        }

        /// <inheritdoc />
        public bool Delete(ContentHash contentHash)
        {
            var hashString = contentHash.ToString();
            var subPath = GetSubPath(hashString);

            var files = Directory.GetFiles(subPath, hashString.Substring(HashPrefixLength) + ".*", SearchOption.TopDirectoryOnly);

            if (files.Length == 0)
            {
                return false;
            }

            foreach (var file in files)
            {
                // Remove the read-only flag from the file
                var attributes = File.GetAttributes(file);
                File.SetAttributes(file, attributes & ~FileAttributes.ReadOnly);

                // Delete the file
                File.Delete(file);
            }

            return true;
        }

        private void GetPaths(ContentHash contentHash, string encodingName, out string subPath, out string contentPath)
        {
            var hashString = contentHash.ToString();
            subPath = GetSubPath(hashString);
            contentPath = Path.Combine(subPath, hashString.Substring(HashPrefixLength));
            if (encodingName != null)
            {
                contentPath += "." + encodingName;
            }
        }

        private string GetContentPath(ContentHash contentHash, string encodingName = null)
        {
            var hashString = contentHash.ToString();
            var subPath = GetSubPath(hashString);
            var contentPath = Path.Combine(subPath, hashString.Substring(HashPrefixLength));
            return encodingName != null
                ? contentPath + "." + encodingName
                : contentPath;
        }

        private string GetSubPath(string hashString) => Path.Combine(_contentPath, hashString.Substring(0, HashPrefixLength));
    }
}
