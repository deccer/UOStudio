using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Graphics
{
    public interface IBuffer : IDisposable
    {
        int Stride { get; }

        int Count { get; }

        void Bind(GL.BufferTargetARB bufferTarget);

        void BindAs(GL.BufferTargetARB bufferTarget, uint slot);

        void Resize(int newSize);

        void Update(IntPtr dataPtr, int size, int offset);

        void Update<T>(T item, int offset)
            where T : unmanaged;

        void Update<T>(T[] data, int offset)
            where T : unmanaged;
    }
}