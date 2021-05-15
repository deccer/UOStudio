using System;
using System.IO;

namespace UOStudio.Common.Core.HashStorage
{
    [Flags]
    public enum ReadOptions
    {
        /// <summary>
        /// Indicates no additional options should be used when reading content from the store.
        /// </summary>
        None = FileOptions.None,

        /// <summary>
        /// Indicates that data will be read sequentially from beginning to end. The system may
        /// use this information to optimise caching.
        /// </summary>
        SequentialScan = FileOptions.SequentialScan,

        /// <summary>
        /// Indicates that data contents will be accessed randomly. The system may use this
        /// information to optimise caching.
        /// </summary>
        RandomAccess = FileOptions.RandomAccess,

        /// <summary>
        /// Indicates that content will be read asynchronously via <see cref="Stream.ReadAsync(byte[],int,int)"/>.
        /// </summary>
        Asynchronous = FileOptions.Asynchronous
    }
}
