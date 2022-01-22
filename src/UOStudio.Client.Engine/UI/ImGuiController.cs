using System.Diagnostics;
using System.Runtime.CompilerServices;
using ImGuiNET;
using UOStudio.Client.Engine.Graphics;
using UOStudio.Client.Engine.Input;
using UOStudio.Client.Engine.Mathematics;
using UOStudio.Client.Engine.Native.OpenGL;
using Num = System.Numerics;
using Vector2 = UOStudio.Client.Engine.Mathematics.Vector2;

namespace UOStudio.Client.Engine.UI
{
    public class ImGuiController : IDisposable
    {
        private bool _frameBegun;

        private uint _vertexArray;
        private uint _vertexBuffer;
        private int _vertexBufferSize;
        private uint _indexBuffer;
        private int _indexBufferSize;

        private ITexture _fontTexture;
        private IShader _shader;

        private readonly IGraphicsDevice _graphicsDevice;

        private int _windowWidth;
        private int _windowHeight;

        private int _scrollWheelValue;

        private Vector2 _scaleFactor = Vector2.One;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CreateBuffer(string name, out uint buffer)
        {
            buffer = GL.CreateBuffer();
            GL.ObjectLabel(GL.ObjectIdentifier.Buffer, buffer, $"ImGuiBuffer: {name}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CreateVertexBuffer(string name, out uint buffer)
            => CreateBuffer($"ImGuiVBO: {name}", out buffer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CreateElementBuffer(string name, out uint buffer)
            => CreateBuffer($"ImGuiEBO: {name}", out buffer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CreateVertexArray(string name, out uint vao)
        {
            vao = GL.CreateVertexArray();
            GL.ObjectLabel(GL.ObjectIdentifier.VertexArray, vao, $"ImGuiVAO: {name}");
        }

        public ImGuiController(IGraphicsDevice graphicsDevice, int width, int height)
        {
            _graphicsDevice = graphicsDevice;
            _windowWidth = width;
            _windowHeight = height;

            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            var io = ImGui.GetIO();
            io.ConfigFlags = ImGuiConfigFlags.DockingEnable;
            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

            io.Fonts.AddFontFromFileTTF("Fonts/Kufam-Regular.ttf", 28);
            io.DisplayFramebufferScale = new Num.Vector2(1.0f, 1.0f);
            var style = ImGui.GetStyle();
            //SetStyleDarker(style);
            SetStylePurple(style);
            style.Colors[(int)ImGuiCol.DockingEmptyBg] = Num.Vector4.Zero;

            CreateDeviceResources();
            SetKeyMappings();
            SetPerFrameImGuiData(1f / 60f);
        }

        public void WindowResized(int width, int height)
        {
            _windowWidth = width;
            _windowHeight = height;
        }

        private void DestroyDeviceObjects()
        {
            Dispose();
        }

        private void CreateDeviceResources()
        {
            CreateVertexArray("ImGui", out _vertexArray);

            _vertexBufferSize = 8192;
            _indexBufferSize = 2048;

            CreateVertexBuffer("ImGui", out _vertexBuffer);
            CreateElementBuffer("ImGui", out _indexBuffer);
            GL.NamedBufferData(_vertexBuffer, _vertexBufferSize, IntPtr.Zero, GL.VertexBufferObjectUsage.DynamicDraw);
            GL.NamedBufferData(_indexBuffer, _indexBufferSize, IntPtr.Zero, GL.VertexBufferObjectUsage.DynamicDraw);

            RecreateFontDeviceTexture();

            var vertexSource = @"#version 460 core
#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_explicit_uniform_location : enable

layout(location = 0) in vec2 in_position;
layout(location = 1) in vec2 in_uv;
layout(location = 2) in vec4 in_color;

out gl_PerVertex
{
    vec4 gl_Position;
};
layout(location = 1) out vec4 fs_color;
layout(location = 2) out vec2 fs_uv;

layout(location = 0) uniform mat4 u_projection;

void main()
{
    gl_Position = u_projection * vec4(in_position, 0, 1);
    fs_color = in_color;
    fs_uv = in_uv;
}";
            var fragmentSource = @"#version 460 core
#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_explicit_uniform_location : enable

layout(location = 1) in vec4 fs_color;
layout(location = 2) in vec2 fs_uv;

layout(location = 0) out vec4 out_color;

layout(binding = 0) uniform sampler2D t_font;

void main()
{
    out_color = fs_color * texture(t_font, fs_uv);
}";
            var shaderResult = _graphicsDevice.CreateShaderProgramFromSources("ImGui", vertexSource, fragmentSource);
            if (shaderResult.IsSuccess)
            {
                _shader = shaderResult.Value;
            }
            else
            {
                Debugger.Break();
            }
            
            GL.VertexArrayVertexBuffer(_vertexArray, 0, _vertexBuffer, IntPtr.Zero, Unsafe.SizeOf<ImDrawVert>());
            GL.VertexArrayElementBuffer(_vertexArray, _indexBuffer);

            GL.EnableVertexArrayAttrib(_vertexArray, 0);
            GL.VertexArrayAttribBinding(_vertexArray, 0, 0);
            GL.VertexArrayAttribFormat(_vertexArray, 0, 2, GL.VertexAttribType.Float, false, 0);

            GL.EnableVertexArrayAttrib(_vertexArray, 1);
            GL.VertexArrayAttribBinding(_vertexArray, 1, 0);
            GL.VertexArrayAttribFormat(_vertexArray, 1, 2, GL.VertexAttribType.Float, false, 8);

            GL.EnableVertexArrayAttrib(_vertexArray, 2);
            GL.VertexArrayAttribBinding(_vertexArray, 2, 0);
            GL.VertexArrayAttribFormat(_vertexArray, 2, 4, GL.VertexAttribType.UnsignedByte, true, 16);
        }

        public void BeginLayout()
        {
            _frameBegun = true;
            ImGui.NewFrame();
        }

        public void EndLayout()
        {
            if (_frameBegun)
            {
                _frameBegun = false;
                ImGui.Render();
                RenderDrawData(ImGui.GetDrawData());
            }
        }

        public void Update(float deltaSeconds)
        {
            SetPerFrameImGuiData(deltaSeconds);
            UpdateImGuiInput();
        }

        private void RecreateFontDeviceTexture()
        {
            var io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out var width, out var height, out var bytesPerPixel);

            _fontTexture = _graphicsDevice.CreateTexture(
                "ImGuiFont",
                width,
                height,
                TextureFormat.Rgba8,
                MagFilter.Linear,
                MinFilter.Linear,
                WrapMode.Clamp, pixels);

            io.Fonts.SetTexID((IntPtr)(uint)(Texture)_fontTexture);
            io.Fonts.ClearTexData();
        }

        private void SetPerFrameImGuiData(float deltaSeconds)
        {
            var io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(
                _windowWidth / _scaleFactor.X,
                _windowHeight / _scaleFactor.Y);
            io.DisplayFramebufferScale = new System.Numerics.Vector2(_scaleFactor.X, _scaleFactor.Y);
            io.DeltaTime = deltaSeconds;
        }

        private readonly List<char> _pressedChars = new List<char>();

        private void UpdateImGuiInput()
        {
            var io = ImGui.GetIO();

            var mouseState = Mouse.GetState();

            io.MouseDown[0] = mouseState.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouseState.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = mouseState.MiddleButton == ButtonState.Pressed;

            var screenPoint = new Point(mouseState.X, mouseState.Y);
            var point = screenPoint;
            io.MousePos = new System.Numerics.Vector2(point.X, point.Y);
            var scrollDelta = mouseState.ScrollWheelValue - _scrollWheelValue;
            io.MouseWheel = scrollDelta > 0
                ? 1
                : scrollDelta < 0
                    ? -1
                    : 0;
            _scrollWheelValue = mouseState.ScrollWheelValue;

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (key == Keys.None)
                {
                    continue;
                }
                io.KeysDown[(int)key] = Keyboard.GetKey(key);
            }

            foreach (var c in _pressedChars)
            {
                io.AddInputCharacter(c);
            }
            _pressedChars.Clear();

            io.KeyCtrl = Keyboard.GetKey(Keys.LeftControl) || Keyboard.GetKey(Keys.RightControl);
            io.KeyAlt = Keyboard.GetKey(Keys.LeftAlt) || Keyboard.GetKey(Keys.RightAlt);
            io.KeyShift = Keyboard.GetKey(Keys.LeftShift) || Keyboard.GetKey(Keys.RightShift);
            io.KeySuper = Keyboard.GetKey(Keys.LeftWindows) || Keyboard.GetKey(Keys.RightWindows);
        }

        internal void PressChar(char keyChar)
        {
            _pressedChars.Add(keyChar);
        }

        internal void MouseScroll(Vector2 offset)
        {
            var io = ImGui.GetIO();

            io.MouseWheel = offset.Y;
            io.MouseWheelH = offset.X;
        }

        private static void SetKeyMappings()
        {
            var io = ImGui.GetIO();
            io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown;
            io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home;
            io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Back;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;
            io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A;
            io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C;
            io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V;
            io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z;
        }

        private unsafe void RenderDrawData(ImDrawDataPtr drawDataPtr)
        {
            if (drawDataPtr.CmdListsCount == 0)
            {
                return;
            }

            for (var i = 0; i < drawDataPtr.CmdListsCount; i++)
            {
                var commandList = drawDataPtr.CmdListsRange[i];
                var vertexSize = commandList.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
                if (vertexSize > _vertexBufferSize)
                {
                    var newSize = (int)Math.Max(_vertexBufferSize * 1.5f, vertexSize);
                    GL.NamedBufferData(_vertexBuffer, newSize, IntPtr.Zero, GL.VertexBufferObjectUsage.DynamicDraw);
                    _vertexBufferSize = newSize;
                }

                var indexSize = commandList.IdxBuffer.Size * sizeof(ushort);
                if (indexSize > _indexBufferSize)
                {
                    var newSize = (int)Math.Max(_indexBufferSize * 1.5f, indexSize);
                    GL.NamedBufferData(_indexBuffer, newSize, IntPtr.Zero, GL.VertexBufferObjectUsage.DynamicDraw);
                    _indexBufferSize = newSize;
                }
            }

            var io = ImGui.GetIO();
            var mvp = Matrix.OrthoOffCenterRH(
                0.0f,
                io.DisplaySize.X,
                io.DisplaySize.Y,
                0.0f,
                -1.0f,
                1.0f);

            _shader.Bind();
            _shader.SetVertexUniform("u_projection", mvp);

            GL.BindTextureUnit(0, (Texture)_fontTexture);
            GL.BindVertexArray(_vertexArray);

            drawDataPtr.ScaleClipRects(io.DisplayFramebufferScale);

            GL.Enable(GL.EnableCap.Blend);
            GL.Enable(GL.EnableCap.ScissorTest);
            GL.BlendEquation(GL.BlendEquationMode.FuncAdd);
            GL.BlendFunc(GL.BlendingFactor.SrcAlpha, GL.BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(GL.EnableCap.CullFace);
            GL.Disable(GL.EnableCap.DepthTest);

            for (var n = 0; n < drawDataPtr.CmdListsCount; n++)
            {
                var commandList = drawDataPtr.CmdListsRange[n];

                GL.NamedBufferSubData(_vertexBuffer, 0, commandList.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), commandList.VtxBuffer.Data);
                GL.NamedBufferSubData(_indexBuffer, 0, commandList.IdxBuffer.Size * sizeof(ushort), commandList.IdxBuffer.Data);
                var vertexOffset = 0;
                var indexOffset = 0;

                for (var commandIndex = 0; commandIndex < commandList.CmdBuffer.Size; commandIndex++)
                {
                    var drawCmdPtr = commandList.CmdBuffer[commandIndex];
                    if (drawCmdPtr.UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }

                    GL.ActiveTexture(GL.TextureUnit.Texture0);
                    GL.BindTexture(GL.TextureTarget.Texture2d, (uint)drawCmdPtr.TextureId);

                    var clip = drawCmdPtr.ClipRect;
                    GL.Scissor((int)clip.X, _windowHeight - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

                    if ((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                    {
                        GL.DrawElementsBaseVertex(GL.PrimitiveType.Triangles, (int)drawCmdPtr.ElemCount, GL.DrawElementsType.UnsignedShort, (indexOffset * sizeof(ushort)), vertexOffset);
                    }
                    else
                    {
                        GL.DrawElements(GL.PrimitiveType.Triangles, (int)drawCmdPtr.ElemCount, GL.DrawElementsType.UnsignedShort, (int)drawCmdPtr.IdxOffset * sizeof(ushort));
                    }

                    indexOffset += (int)drawCmdPtr.ElemCount;
                }

                vertexOffset += commandList.VtxBuffer.Size;
            }

            GL.Disable(GL.EnableCap.Blend);
            GL.Disable(GL.EnableCap.ScissorTest);
        }

        public void Dispose()
        {
            _fontTexture.Dispose();
            _shader.Dispose();
        }

        private void SetStylePurple(ImGuiStylePtr style)
        { 
            style.Colors[(int)ImGuiCol.Text]                   = new Num.Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TextDisabled]           = new Num.Vector4(0.50f, 0.50f, 0.50f, 1.00f);
            style.Colors[(int)ImGuiCol.WindowBg]               = new Num.Vector4(0.08f, 0.08f, 0.09f, 1.00f);
            style.Colors[(int)ImGuiCol.ChildBg]                = new Num.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            style.Colors[(int)ImGuiCol.PopupBg]                = new Num.Vector4(0.15f, 0.15f, 0.17f, 0.94f);
            style.Colors[(int)ImGuiCol.Border]                 = new Num.Vector4(0.37f, 0.31f, 0.57f, 1.00f);
            style.Colors[(int)ImGuiCol.BorderShadow]           = new Num.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            style.Colors[(int)ImGuiCol.FrameBg]                = new Num.Vector4(0.24f, 0.22f, 0.33f, 1.00f);
            style.Colors[(int)ImGuiCol.FrameBgHovered]         = new Num.Vector4(0.41f, 0.39f, 0.50f, 0.40f);
            style.Colors[(int)ImGuiCol.FrameBgActive]          = new Num.Vector4(0.41f, 0.40f, 0.50f, 0.62f);
            style.Colors[(int)ImGuiCol.TitleBg]                = new Num.Vector4(0.12f, 0.11f, 0.14f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgActive]          = new Num.Vector4(0.12f, 0.11f, 0.14f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed]       = new Num.Vector4(0.00f, 0.00f, 0.00f, 0.51f);
            style.Colors[(int)ImGuiCol.MenuBarBg]              = new Num.Vector4(0.24f, 0.22f, 0.33f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarBg]            = new Num.Vector4(0.02f, 0.02f, 0.02f, 0.53f);
            style.Colors[(int)ImGuiCol.ScrollbarGrab]          = new Num.Vector4(0.31f, 0.31f, 0.31f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered]   = new Num.Vector4(0.41f, 0.41f, 0.41f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive]    = new Num.Vector4(0.51f, 0.51f, 0.51f, 1.00f);
            style.Colors[(int)ImGuiCol.CheckMark]              = new Num.Vector4(0.60f, 0.56f, 0.77f, 1.00f);
            style.Colors[(int)ImGuiCol.SliderGrab]             = new Num.Vector4(0.56f, 0.54f, 0.66f, 0.40f);
            style.Colors[(int)ImGuiCol.SliderGrabActive]       = new Num.Vector4(0.76f, 0.73f, 0.88f, 0.40f);
            style.Colors[(int)ImGuiCol.Button]                 = new Num.Vector4(0.24f, 0.22f, 0.33f, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonHovered]          = new Num.Vector4(0.32f, 0.29f, 0.44f, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonActive]           = new Num.Vector4(0.21f, 0.20f, 0.26f, 0.40f);
            style.Colors[(int)ImGuiCol.Header]                 = new Num.Vector4(0.31f, 0.29f, 0.37f, 0.40f);
            style.Colors[(int)ImGuiCol.HeaderHovered]          = new Num.Vector4(0.47f, 0.45f, 0.57f, 0.40f);
            style.Colors[(int)ImGuiCol.HeaderActive]           = new Num.Vector4(0.21f, 0.20f, 0.25f, 0.40f);
            style.Colors[(int)ImGuiCol.Separator]              = new Num.Vector4(0.37f, 0.31f, 0.57f, 1.00f);
            style.Colors[(int)ImGuiCol.SeparatorHovered]       = new Num.Vector4(0.10f, 0.40f, 0.75f, 0.78f);
            style.Colors[(int)ImGuiCol.SeparatorActive]        = new Num.Vector4(0.10f, 0.40f, 0.75f, 1.00f);
            style.Colors[(int)ImGuiCol.ResizeGrip]             = new Num.Vector4(0.47f, 0.45f, 0.57f, 0.74f);
            style.Colors[(int)ImGuiCol.ResizeGripHovered]      = new Num.Vector4(0.59f, 0.57f, 0.71f, 0.74f);
            style.Colors[(int)ImGuiCol.ResizeGripActive]       = new Num.Vector4(0.35f, 0.33f, 0.41f, 0.74f);
            style.Colors[(int)ImGuiCol.Tab]                    = new Num.Vector4(0.24f, 0.22f, 0.33f, 1.00f);
            style.Colors[(int)ImGuiCol.TabHovered]             = new Num.Vector4(0.38f, 0.34f, 0.53f, 1.00f);
            style.Colors[(int)ImGuiCol.TabActive]              = new Num.Vector4(0.24f, 0.22f, 0.33f, 1.00f);
            style.Colors[(int)ImGuiCol.TabUnfocused]           = new Num.Vector4(0.27f, 0.26f, 0.32f, 0.40f);
            style.Colors[(int)ImGuiCol.TabUnfocusedActive]     = new Num.Vector4(0.42f, 0.39f, 0.57f, 0.40f);
            //style.Colors[(int)ImGuiCol.TabUnfocusedBorder]     = new Num.Vector4(0.11f, 0.09f, 0.17f, 1.00f);
            style.Colors[(int)ImGuiCol.DockingPreview]         = new Num.Vector4(0.58f, 0.54f, 0.80f, 0.78f);
            style.Colors[(int)ImGuiCol.DockingEmptyBg]         = new Num.Vector4(0.12f, 0.11f, 0.14f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLines]              = new Num.Vector4(0.61f, 0.61f, 0.61f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLinesHovered]       = new Num.Vector4(1.00f, 0.43f, 0.35f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogram]          = new Num.Vector4(0.90f, 0.70f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogramHovered]   = new Num.Vector4(1.00f, 0.60f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TextSelectedBg]         = new Num.Vector4(0.26f, 0.59f, 0.98f, 0.35f);
            style.Colors[(int)ImGuiCol.DragDropTarget]         = new Num.Vector4(1.00f, 1.00f, 0.00f, 0.90f);
            style.Colors[(int)ImGuiCol.NavHighlight]           = new Num.Vector4(0.26f, 0.59f, 0.98f, 1.00f);
            style.Colors[(int)ImGuiCol.NavWindowingHighlight]  = new Num.Vector4(1.00f, 1.00f, 1.00f, 0.70f);
            style.Colors[(int)ImGuiCol.NavWindowingDimBg]      = new Num.Vector4(0.80f, 0.80f, 0.80f, 0.20f);
            style.Colors[(int)ImGuiCol.ModalWindowDimBg]       = new Num.Vector4(0.80f, 0.80f, 0.80f, 0.35f);
            style.WindowBorderSize = 1.0f;
            style.PopupBorderSize = 0.0f;
            style.FrameRounding = 6.0f;
            style.PopupRounding = 1.0f;
            style.WindowRounding = 3.0f;
            style.ScrollbarRounding = 3.0f;
            style.GrabRounding = 2.0f;
            style.ChildBorderSize = 0.0f;
            style.TabBorderSize = 2.0f;
            style.AntiAliasedLinesUseTex = false;
        }

        private void SetStyleDarker(ImGuiStylePtr style)
        {
            style.WindowPadding = new Num.Vector2(12, 12);
            style.WindowRounding = 5.0f;
            style.FramePadding = new Num.Vector2(4, 4);
            style.FrameRounding = 4.0f;
            style.ItemSpacing = new Num.Vector2(8, 8);
            style.ItemInnerSpacing = new Num.Vector2(4, 4);
            style.IndentSpacing = 16.0f;
            style.ScrollbarSize = 16.0f;
            style.ScrollbarRounding = 8.0f;
            style.GrabMinSize = 4.0f;
            style.GrabRounding = 3.0f;

            style.Colors[(int)ImGuiCol.Text] = new Num.Vector4(0.80f, 0.80f, 0.83f, 1.00f);
            style.Colors[(int)ImGuiCol.TextDisabled] = new Num.Vector4(0.24f, 0.23f, 0.29f, 1.00f);
            style.Colors[(int)ImGuiCol.WindowBg] = new Num.Vector4(0.06f, 0.05f, 0.07f, 1.00f);
            style.Colors[(int)ImGuiCol.ChildBg] = new Num.Vector4(0.07f, 0.07f, 0.09f, 1.00f);
            style.Colors[(int)ImGuiCol.PopupBg] = new Num.Vector4(0.07f, 0.07f, 0.09f, 1.00f);
            style.Colors[(int)ImGuiCol.Border] = new Num.Vector4(0.20f, 0.20f, 0.23f, 0.88f);
            style.Colors[(int)ImGuiCol.BorderShadow] = new Num.Vector4(0.92f, 0.91f, 0.88f, 0.00f);
            style.Colors[(int)ImGuiCol.FrameBg] = new Num.Vector4(0.10f, 0.09f, 0.12f, 1.00f);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = new Num.Vector4(0.24f, 0.23f, 0.29f, 1.00f);
            style.Colors[(int)ImGuiCol.FrameBgActive] = new Num.Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBg] = new Num.Vector4(0.10f, 0.09f, 0.12f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Num.Vector4(1.00f, 0.98f, 0.95f, 0.75f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new Num.Vector4(0.07f, 0.07f, 0.09f, 1.00f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = new Num.Vector4(0.10f, 0.09f, 0.12f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = new Num.Vector4(0.10f, 0.09f, 0.12f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Num.Vector4(0.80f, 0.80f, 0.83f, 0.31f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Num.Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Num.Vector4(0.06f, 0.05f, 0.07f, 1.00f);
            style.Colors[(int)ImGuiCol.CheckMark] = new Num.Vector4(0.80f, 0.80f, 0.83f, 0.31f);
            style.Colors[(int)ImGuiCol.SliderGrab] = new Num.Vector4(0.80f, 0.80f, 0.83f, 0.31f);
            style.Colors[(int)ImGuiCol.SliderGrabActive] = new Num.Vector4(0.06f, 0.05f, 0.07f, 1.00f);
            style.Colors[(int)ImGuiCol.Button] = new Num.Vector4(0.10f, 0.09f, 0.12f, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new Num.Vector4(0.24f, 0.23f, 0.29f, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonActive] = new Num.Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            style.Colors[(int)ImGuiCol.Header] = new Num.Vector4(0.10f, 0.09f, 0.12f, 1.00f);
            style.Colors[(int)ImGuiCol.HeaderHovered] = new Num.Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            style.Colors[(int)ImGuiCol.HeaderActive] = new Num.Vector4(0.06f, 0.05f, 0.07f, 1.00f);
            style.Colors[(int)ImGuiCol.ResizeGrip] = new Num.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Num.Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            style.Colors[(int)ImGuiCol.ResizeGripActive] = new Num.Vector4(0.06f, 0.05f, 0.07f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLines] = new Num.Vector4(0.40f, 0.39f, 0.38f, 0.63f);
            style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Num.Vector4(0.25f, 1.00f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogram] = new Num.Vector4(0.40f, 0.39f, 0.38f, 0.63f);
            style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Num.Vector4(0.25f, 1.00f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TextSelectedBg] = new Num.Vector4(0.25f, 1.00f, 0.00f, 0.43f);
        }
    }
}
