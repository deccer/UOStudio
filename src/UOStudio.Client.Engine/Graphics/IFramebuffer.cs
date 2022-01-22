using UOStudio.Client.Engine.Mathematics;
using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Graphics
{
    public interface IFramebuffer : IDisposable
    {
        int Width { get; }

        int Height { get; }

        void Bind();

        void BlitTo(
            uint targetFramebuffer,
            int sourceWidth,
            int sourceHeight,
            int targetWidth,
            int targetHeight,
            GL.BlitFramebufferFilter blitFramebufferFilter = GL.BlitFramebufferFilter.Nearest);

        void Clear(int colorIndex, Vector2 clearColor);

        void Clear(int colorIndex, Vector3 clearColor);

        void Clear(int colorIndex, Vector4 clearColor);

        void Clear(int colorIndex, Int4 clearColor);

        void ClearDepth(float depthValue);

        void ClearDepthStencil(float depthValue, int stencilValue);

        void DrawTo(params GL.ColorBuffer[] targets);
    }
}