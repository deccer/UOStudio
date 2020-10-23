using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Serilog;
using UOStudio.Core;

namespace UOStudio.Client.Engine.Ultima
{
    internal unsafe class UltimaFile : DataReader
    {
        private readonly ILogger _logger;
        private protected MemoryMappedViewAccessor _accessor;
        private protected MemoryMappedFile _file;

        public UltimaFile([NotNull] ILogger logger, string filepath, bool loadFile = false)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            FilePath = filepath;

            if (loadFile)
            {
                Load();
            }
        }

        public string FilePath { get; }

        protected virtual void Load()
        {
            _logger.Debug($"Loading mulFileName:\t\t{FilePath}");

            var fileInfo = new FileInfo(FilePath);
            if (!fileInfo.Exists)
            {
                Log.Error($"File {FilePath} not exists.");
                return;
            }

            var size = fileInfo.Length;
            if (size > 0)
            {
                _file = MemoryMappedFile.CreateFromFile(
                    File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), null, 0,
                    MemoryMappedFileAccess.Read, HandleInheritability.None, false);

                _accessor = _file.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);

                byte* ptr = null;

                try
                {
                    _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                    SetData(ptr, (long) _accessor.SafeMemoryMappedViewHandle.ByteLength);
                }
                catch
                {
                    _accessor.SafeMemoryMappedViewHandle.ReleasePointer();

                    throw new Exception("Something goes wrong...");
                }
            }
            else
            {
                _logger.Error($"{FilePath}  size must be > 0");
            }
        }

        public virtual void FillEntries(ref UltimaFileIndex[] entries)
        {
        }

        public virtual void Dispose()
        {
            _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            _accessor.Dispose();
            _file.Dispose();
            _logger.Debug($"Unloaded:\t\t{FilePath}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Fill(ref byte[] buffer, int count)
        {
            var ptr = (byte*) PositionAddress;

            for (var i = 0; i < count; i++)
            {
                buffer[i] = ptr[i];
            }

            Position += count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T[] ReadArray<T>(int count) where T : struct
        {
            var t = ReadArray<T>(Position, count);
            Position += MemoryManager.SizeOf<T>() * count;

            return t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T[] ReadArray<T>(long position, int count) where T : struct
        {
            var array = new T[count];
            _accessor.ReadArray(position, array, 0, count);

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T ReadStruct<T>(long position) where T : struct
        {
            _accessor.Read(position, out T s);

            return s;
        }
    }
}
