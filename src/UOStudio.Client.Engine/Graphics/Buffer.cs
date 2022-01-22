using System.Runtime.CompilerServices;
using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Graphics
{
    internal abstract class Buffer : IBuffer
    {
        protected readonly uint Id;

        protected Buffer()
        {
            Id = GL.CreateBuffer();
        }

        public int Stride { get; protected set; }

        public int Count { get; protected set; }

        public void Bind(GL.BufferTargetARB bufferTarget)
        {
            GL.BindBuffer(bufferTarget, Id);
        }

        public void BindAs(GL.BufferTargetARB bufferTarget, uint slot)
        {
            GL.BindBufferBase(bufferTarget, slot, Id);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(Id);
        }

        public void Resize(int newSize)
        {
            GL.NamedBufferData(Id, newSize, IntPtr.Zero, GL.VertexBufferObjectUsage.DynamicDraw);
        }

        public void Update(IntPtr dataPtr, int size, int offset)
        {
            GL.NamedBufferSubData(Id, offset, size, dataPtr);
        }

        public void Update<T>(T data, int offset)
            where T : unmanaged
        {
            unsafe
            {
                GL.NamedBufferSubData(Id, offset * sizeof(T), new [] { data });
                Count = 1;
            }
        }

        public void Update<T>(T[] data, int offset)
            where T : unmanaged
        {
            unsafe
            {
                GL.NamedBufferSubData(Id, offset * sizeof(T), data);
                Count = data.Length;
            }
        }

        public static implicit operator uint(Buffer buffer)
        {
            return buffer.Id;
        }
    }

    internal class Buffer<T> : Buffer
        where T : unmanaged
    {
        private Buffer(string name = "")
        {
            var label = $"B_{typeof(T).Name}";
            if (!string.IsNullOrEmpty(name))
            {
                label += $"_{name}";
            }
            GL.ObjectLabel(GL.ObjectIdentifier.Buffer, Id, label);
        }

        internal Buffer(string name, T data)
            : this(name)
        {
            GL.NamedBufferStorage(Id, new [] { data }, GL.BufferStorageMask.DynamicStorageBit);
            Stride = Unsafe.SizeOf<T>();
            Count = 1;
        }

        internal Buffer(string name, T[] data)
            : this(name)
        {
            GL.NamedBufferStorage(Id, data, GL.BufferStorageMask.DynamicStorageBit);
            Stride = Unsafe.SizeOf<T>();
            Count = data.Length;
        }

        internal Buffer(string name, int size)
            : this(name)
        {
            Stride = Unsafe.SizeOf<T>();
            unsafe
            {
                GL.NamedBufferStorage(Id, size * sizeof(T), IntPtr.Zero, GL.BufferStorageMask.DynamicStorageBit);
            }
        }
    }
}
