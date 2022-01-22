using UOStudio.Client.Engine.Mathematics;
using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Graphics
{
    internal sealed class Framebuffer : IFramebuffer
    {
        private readonly uint _id;

        private Framebuffer(string label)
        {
            _id = GL.CreateFramebuffer();
            GL.ObjectLabel(GL.ObjectIdentifier.Framebuffer, _id, label);
        }

        internal Framebuffer(
            string label,
            ITexture[] colorAttachments,
            ITexture depthAttachment = null)
            : this(label)
        {
            var drawBuffers = new GL.ColorBuffer[colorAttachments.Length];
            if (colorAttachments.Any())
            {
                for (var i = 0; i < colorAttachments.Length; i++)
                {
                    if (i == 0)
                    {
                        Width = colorAttachments[i].Width;
                        Height = colorAttachments[i].Height;
                    }

                    var attachment = (GL.FramebufferAttachment)((int)GL.FramebufferAttachment.ColorAttachment0 + i);
                    GL.NamedFramebufferTexture(_id, attachment, (Texture)colorAttachments[i], 0);
                    drawBuffers[i] = (GL.ColorBuffer)((int)GL.ColorBuffer.ColorAttachment0 + i);
                }
            }

            if (depthAttachment != null)
            {
                GL.NamedFramebufferTexture(_id, GL.FramebufferAttachment.DepthAttachment, (Texture)depthAttachment, 0);
            }

            if (drawBuffers.Any())
            {
                GL.NamedFramebufferDrawBuffers(_id, drawBuffers.Length, drawBuffers[0]);
            }
            else
            {
                //GL.NamedFramebufferDrawBuffer(_id, GL.ColorBuffer.None);
                //GL.NamedFramebufferReadBuffer(_id, GL.ColorBuffer.None);
            }

            var namedFramebufferStatus = GL.CheckNamedFramebufferStatus(_id, GL.FramebufferTarget.Framebuffer);
            if (namedFramebufferStatus != GL.FramebufferStatus.FramebufferComplete)
            {
                throw new InvalidOperationException("Framebuffer Incomplete");
            }
        }

        public int Width { get; }

        public int Height { get; }

        public void Dispose()
        {
            GL.DeleteFramebuffer(_id);
        }

        public void Bind()
        {
            GL.BindFramebuffer(GL.FramebufferTarget.Framebuffer, _id);
        }

        public void DrawTo(params GL.ColorBuffer[] targets)
        {
            GL.NamedFramebufferDrawBuffers(_id, targets.Length, targets[0]);
        }

        public void BlitTo(
            uint targetFramebuffer,
            int sourceWidth,
            int sourceHeight,
            int targetWidth,
            int targetHeight,
            GL.BlitFramebufferFilter blitFramebufferFilter = GL.BlitFramebufferFilter.Nearest)
        {
            GL.BlitNamedFramebuffer(_id, targetFramebuffer, 0, 0,
                sourceWidth, sourceHeight, 0, 0, targetWidth, targetHeight,
                GL.ClearBufferMask.ColorBufferBit, blitFramebufferFilter);
        }

        public void Clear(int colorIndex, Int4 clearColor)
        {
            GL.ClearNamedFramebufferi(_id, GL.Buffer.Color, colorIndex, clearColor.X);
        }

        public void Clear(int colorIndex, Vector2 clearColor)
        {
            GL.ClearNamedFramebufferf(_id, GL.Buffer.Color, colorIndex, clearColor.X);
        }

        public void Clear(int colorIndex, Vector3 clearColor)
        {
            GL.ClearNamedFramebufferf(_id, GL.Buffer.Color, colorIndex, clearColor.X);
        }

        public void Clear(int colorIndex, Vector4 clearColor)
        {
            GL.ClearNamedFramebufferf(_id, GL.Buffer.Color, colorIndex, clearColor.X);
        }

        public void ClearDepth(float depthValue)
        {
            GL.ClearNamedFramebufferf(_id, GL.Buffer.Depth, 0, depthValue);
        }

        public void ClearDepthStencil(float depthValue, int stencilValue)
        {
            GL.ClearNamedFramebufferfi(_id, GL.Buffer.DepthStencil, 0, depthValue, stencilValue);
        }

        public static implicit operator uint(Framebuffer framebuffer)
        {
            return framebuffer._id;
        }
    }
}