using System.Runtime.InteropServices;

namespace UOStudio.Client.Engine.Native.OpenGL
{
    public unsafe partial class GL
    {
        public static void BindRenderbuffer(RenderbufferTarget target, uint renderbuffer)
        {
            _bindRenderbufferDelegate(target, renderbuffer);
        }

        private static delegate* unmanaged<RenderbufferTarget, uint, void> _bindRenderbufferDelegate = &BindRenderbuffer_Lazy;
        [UnmanagedCallersOnly]
        private static void BindRenderbuffer_Lazy(RenderbufferTarget target, uint renderbuffer)
        {
            _bindRenderbufferDelegate =
                (delegate* unmanaged<RenderbufferTarget, uint, void>)Sdl.GetProcAddress("glBindRenderbuffer");
            _bindRenderbufferDelegate(target, renderbuffer);
        }

        public static void DeleteRenderbuffers(int n, uint* renderBuffers)
        {
            _deleteRenderbuffersDelegate(n, renderBuffers);
        }

        private static delegate* unmanaged<int, uint*, void> _deleteRenderbuffersDelegate = &DeleteRenderbuffers_Lazy;
        [UnmanagedCallersOnly]
        private static void DeleteRenderbuffers_Lazy(int n, uint* renderBuffers)
        {
            _deleteRenderbuffersDelegate = (delegate* unmanaged<int, uint*, void>)Sdl.GetProcAddress("glDeleteRenderbuffers");
            _deleteRenderbuffersDelegate(n, renderBuffers);
        }

        public static void GenRenderbuffers(int n, uint* renderBuffers)
        {
            _genRenderbuffersDelegate(n, renderBuffers);
        }

        private static delegate* unmanaged<int, uint*, void> _genRenderbuffersDelegate = &GenRenderbuffers_Lazy;
        [UnmanagedCallersOnly]
        private static void GenRenderbuffers_Lazy(int n, uint* renderBuffers)
        {
            _genRenderbuffersDelegate = (delegate* unmanaged<int, uint*, void>)Sdl.GetProcAddress("glGenRenderbuffers");
            _genRenderbuffersDelegate(n, renderBuffers);
        }

        public static void RenderbufferStorage(
            RenderbufferTarget target,
            SizedInternalFormat internalformat,
            int width,
            int height)
        {
            _renderbufferStorageDelegate(target, internalformat, width, height);
        }

        private static delegate* unmanaged<RenderbufferTarget, SizedInternalFormat, int, int, void> _renderbufferStorageDelegate = &RenderbufferStorage_Lazy;

        [UnmanagedCallersOnly]
        private static void RenderbufferStorage_Lazy(
            RenderbufferTarget target,
            SizedInternalFormat internalformat,
            int width,
            int height)
        {
            _renderbufferStorageDelegate = (delegate* unmanaged<RenderbufferTarget, SizedInternalFormat, int, int, void>)Sdl.GetProcAddress("glRenderbufferStorage");
            _renderbufferStorageDelegate(target, internalformat, width, height);
        }

        public static void FramebufferRenderbuffer(
            FramebufferTarget target,
            FramebufferAttachment attachment,
            RenderbufferTarget renderbufferTarget,
            uint renderbuffer)
        {
            _framebufferRenderbufferDelegate(target, attachment, renderbufferTarget, renderbuffer);
        }

        private static delegate* unmanaged<FramebufferTarget, FramebufferAttachment, RenderbufferTarget, uint, void>
            _framebufferRenderbufferDelegate = &FramebufferRenderbuffer_Lazy;

        [UnmanagedCallersOnly]
        private static void FramebufferRenderbuffer_Lazy(FramebufferTarget target, FramebufferAttachment attachment,
            RenderbufferTarget renderbufferTarget, uint renderbuffer)
        {
            _framebufferRenderbufferDelegate = (delegate* unmanaged<FramebufferTarget, FramebufferAttachment, RenderbufferTarget, uint, void>)Sdl.GetProcAddress("glFramebufferRenderbuffer");
            _framebufferRenderbufferDelegate(target, attachment, renderbufferTarget, renderbuffer);
        }

        public static FramebufferStatus CheckFramebufferStatus(FramebufferTarget target)
        {
            return _checkFramebufferStatusDelegate(target);
        }

        private static delegate* unmanaged<FramebufferTarget, FramebufferStatus> _checkFramebufferStatusDelegate = &CheckFramebufferStatus_Lazy;
        [UnmanagedCallersOnly]
        private static FramebufferStatus CheckFramebufferStatus_Lazy(FramebufferTarget target)
        {
            _checkFramebufferStatusDelegate = (delegate* unmanaged<FramebufferTarget, FramebufferStatus>)Sdl.GetProcAddress("glCheckFramebufferStatus");
            return _checkFramebufferStatusDelegate(target);
        }
    }
}
