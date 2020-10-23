using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace UOStudio.Core
{
    public unsafe class DataReader
    {
        private byte* _data;
        private GCHandle _handle;

        public long Position { get; set; }

        public long Length { get; private set; }

        public IntPtr StartAddress => (IntPtr)_data;

        public IntPtr PositionAddress => (IntPtr)(_data + Position);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseData()
        {
            if (_handle.IsAllocated)
            {
                _handle.Free();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetData(byte* data, long length)
        {
            ReleaseData();

            _data = data;
            Length = length;
            Position = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetData(byte[] data, long length)
        {
            ReleaseData();
            _handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            _data = (byte*)_handle.AddrOfPinnedObject();
            Length = length;
            Position = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetData(IntPtr data, long length)
        {
            SetData((byte*)data, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetData(IntPtr data)
        {
            SetData((byte*)data, Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Seek(long position)
        {
            Position = position;
            EnsureSize(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Seek(int position)
        {
            Position = position;
            EnsureSize(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Skip(int count)
        {
            EnsureSize(count);
            Position += count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadUInt8()
        {
            EnsureSize(1);

            return _data[Position++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadInt8() => (sbyte)ReadUInt8();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool() => ReadUInt8() != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16()
        {
            EnsureSize(2);

            var v = *(short*)(_data + Position);
            Position += 2;

            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
        {
            EnsureSize(2);

            var v = *(ushort*)(_data + Position);
            Position += 2;

            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
        {
            EnsureSize(4);

            var v = *(int*)(_data + Position);

            Position += 4;

            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt()
        {
            EnsureSize(4);

            var v = *(uint*)(_data + Position);
            Position += 4;

            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64()
        {
            EnsureSize(8);

            var v = *(long*)(_data + Position);
            Position += 8;

            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64()
        {
            EnsureSize(8);

            var v = *(ulong*)(_data + Position);
            Position += 8;

            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ReadArray(int count)
        {
            EnsureSize(count);

            var data = new byte[count];

            fixed (byte* ptr = data)
            {
                Buffer.MemoryCopy(&_data[Position], ptr, count, count);
            }

            Position += count;

            return data;
        }

        public string ReadAscii(int size)
        {
            EnsureSize(size);

            var sb = new StringBuilder(size);

            for (var i = 0; i < size; i++)
            {
                var c = (char)ReadUInt8();

                if (c != 0)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureSize(int size)
        {
            if (Position + size > Length)
            {
                throw new IndexOutOfRangeException();
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16Reversed()
        {
            EnsureSize(2);

            return (ushort)((ReadUInt8() << 8) | ReadUInt8());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32Reversed()
        {
            EnsureSize(4);

            return (uint)((ReadUInt8() << 24) | (ReadUInt8() << 16) | (ReadUInt8() << 8) | ReadUInt8());
        }
    }
}
