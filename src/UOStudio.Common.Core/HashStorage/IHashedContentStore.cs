﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace UOStudio.Common.Core.HashStorage
{
    public interface IHashedContentStore
    {
        /// <summary>
        /// Write content to the store, returning its hash.
        /// </summary>
        /// <remarks>
        /// If the store already contains this content, the write is discarded but the hash is
        /// returned, as normal.
        /// <para />
        /// When one or more encodings are provided, the store will compute and persist encoded
        /// versions of the content as well.
        /// <para />
        /// If the store already contains the content, but not in a specified encoding, then
        /// a correspondingly encoded version will be created and persisted.
        /// </remarks>
        /// <param name="stream">A stream from which the data to be written can be read.</param>
        /// <param name="cancellationToken">An optional cancellation token which may be used to cancel the asynchronous write operation.</param>
        /// <param name="encodings">A sequence of encodings to also store this content with. If <c>null</c> or empty, no additional encodings are used.</param>
        /// <returns>An async task, the result of which is the written content's hash.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <c>null</c>.</exception>
        Task<ContentHash> WriteAsync(Stream stream, CancellationToken cancellationToken = new CancellationToken(), IEnumerable<IContentEncoding> encodings = null);

        /// <summary>
        /// Gets a value indicating whether content exists in the store with the specified <paramref name="contentHash"/>.
        /// </summary>
        /// <param name="contentHash">The hash of the content to search for.</param>
        /// <param name="encodingName">Conditions the check on whether the content exists with the specified encoding. If <c>null</c>, the check indicates whether the unencoded content exists in the store.</param>
        /// <returns><c>true</c> if the content exists in the store, otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="contentHash"/> is <c>null</c>.</exception>
        bool Contains(ContentHash contentHash, string encodingName = null);

        /// <summary>
        /// Read content from the store.
        /// </summary>
        /// <remarks>
        /// When <c>true</c> is returned, <paramref name="stream"/> will be non-<c>null</c> and
        /// must be disposed when finished with.
        /// <para />
        /// If <paramref name="encodingName"/> is non-<c>null</c> and the content was not written
        /// explicitly using the encoding, this method will return <c>false</c>.
        /// </remarks>
        /// <param name="contentHash">The hash of the content to read.</param>
        /// <param name="stream">A stream from which the stored content may be read.</param>
        /// <param name="options">Optional parameters to control how data be read from disk.
        /// See the <see cref="ReadOptions"/> enum for further details.</param>
        /// <param name="encodingName">The encoding used when storing the content, or <c>null</c> to access the unencoded content.</param>
        /// <returns><c>true</c> if the content was found, otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="contentHash"/> is <c>null</c>.</exception>
        bool TryRead(ContentHash contentHash, out Stream stream, ReadOptions options = ReadOptions.None, string encodingName = null);

        /// <summary>
        /// Get the length of stored content.
        /// </summary>
        /// <param name="contentHash">The hash of the content to measure.</param>
        /// <param name="length">The length of the content in bytes.</param>
        /// <param name="encodingName">The encoding used when storing the content, or <c>null</c> to access the unencoded content.</param>
        /// <returns><c>true</c> if the requested content exists, otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="contentHash"/> is <c>null</c>.</exception>
        bool TryGetContentLength(ContentHash contentHash, out long length, string encodingName = null);

        /// <summary>
        /// Get an enumeration over all hashes contained within the store.
        /// </summary>
        /// <remarks>
        /// This enumeration is computed lazily by querying the file system and therefore will
        /// not behave deterministically if modified while enumerating.
        /// </remarks>
        /// <returns>An enumeration over all hashes contained within the store.</returns>
        IEnumerable<ContentHash> GetHashes();

        /// <summary>
        /// Attempt to delete an item of content.
        /// </summary>
        /// <remarks>
        /// All encodings of the content will be deleted.
        /// </remarks>
        /// <param name="contentHash">The hash of the content to delete.</param>
        /// <returns><c>true</c> if the content existed and was deleted, otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="contentHash"/> is <c>null</c>.</exception>
        bool Delete(ContentHash contentHash);
    }
}
