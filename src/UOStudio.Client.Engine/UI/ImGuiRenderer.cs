using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Num = System.Numerics;

namespace UOStudio.Client.Engine.UI
{
    public sealed class ImGuiRenderer : IDisposable
    {
        private readonly GraphicsDevice _graphicsDevice;

        private BasicEffect _effect;
        private readonly RasterizerState _rasterizerState;

        private byte[] _vertexData;
        private VertexBuffer _vertexBuffer;
        private int _vertexBufferSize;

        private byte[] _indexData;
        private IndexBuffer _indexBuffer;
        private int _indexBufferSize;

        private readonly Dictionary<IntPtr, Texture2D> _loadedTextures;

        private Texture2D _fontAtlasTexture;
        private int _textureId;
        private IntPtr? _fontAtlasTextureId;
        private int _scrollWheelValue;

        private readonly List<int> _keys = new List<int>();

        public ImGuiRenderer(Game game)
        {
            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            _graphicsDevice = game.GraphicsDevice;

            _loadedTextures = new Dictionary<IntPtr, Texture2D>();

            _rasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                DepthBias = 0,
                FillMode = FillMode.Solid,
                MultiSampleAntiAlias = false,
                ScissorTestEnable = true,
                SlopeScaleDepthBias = 0
            };

            var io = ImGui.GetIO();
            io.ConfigFlags = ImGuiConfigFlags.DockingEnable;
            SetupInput(io);
            //SetRedStyle(ImGui.GetStyle());
        }

        public unsafe void RebuildFontAtlas()
        {
            var io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out var width, out var height, out var bytesPerPixel);

            var pixels = new byte[width * height * bytesPerPixel];
            Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length);

            _fontAtlasTexture?.Dispose();
            _fontAtlasTexture = new Texture2D(_graphicsDevice, width, height, false, SurfaceFormat.Color);
            _fontAtlasTexture.SetData(pixels);

            if (_fontAtlasTextureId.HasValue)
            {
                UnbindTexture(_fontAtlasTextureId.Value);
            }

            _fontAtlasTextureId = BindTexture(_fontAtlasTexture);
            io.Fonts.SetTexID(_fontAtlasTextureId.Value);
            io.Fonts.ClearTexData();
        }

        public IntPtr BindTexture(Texture2D texture)
        {
            var id = new IntPtr(_textureId++);

            _loadedTextures.Add(id, texture);

            return id;
        }

        public void Dispose()
        {
            _fontAtlasTexture?.Dispose();
        }

        public void UnbindTexture(IntPtr textureId)
        {
            _loadedTextures.Remove(textureId);
        }

        public void BeginLayout(GameTime gameTime)
        {
            ImGui.GetIO().DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateInput();

            ImGui.NewFrame();
        }

        public void EndLayout()
        {
            ImGui.Render();

            RenderDrawData(ImGui.GetDrawData());
        }

        protected void SetupInput(ImGuiIOPtr io)
        {
            _keys.Add(io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab);
            _keys.Add(io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left);
            _keys.Add(io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right);
            _keys.Add(io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up);
            _keys.Add(io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down);
            _keys.Add(io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp);
            _keys.Add(io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home);
            _keys.Add(io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Back);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape);
            _keys.Add(io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A);
            _keys.Add(io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C);
            _keys.Add(io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V);
            _keys.Add(io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z);

            TextInputEXT.TextInput += c =>
            {
                if (c == '\t')
                {
                    return;
                }

                ImGui.GetIO().AddInputCharacter(c);
            };

            ImGui.GetIO().Fonts.AddFontDefault();
        }

        protected Effect UpdateEffect(Texture2D texture)
        {
            _effect ??= new BasicEffect(_graphicsDevice);

            var io = ImGui.GetIO();
            var offset = 0f;

            _effect.World = Matrix.Identity;
            _effect.View = Matrix.Identity;
            _effect.Projection = Matrix.CreateOrthographicOffCenter(
                offset,
                io.DisplaySize.X + offset,
                io.DisplaySize.Y + offset,
                offset,
                -1f,
                1f
            );
            _effect.TextureEnabled = true;
            _effect.Texture = texture;
            _effect.VertexColorEnabled = true;

            return _effect;
        }

        public void UpdateInput()
        {
            var io = ImGui.GetIO();

            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            for (var i = 0; i < _keys.Count; i++)
            {
                io.KeysDown[_keys[i]] = keyboard.IsKeyDown((Keys)_keys[i]);
            }

            io.KeyShift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            io.KeyCtrl = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
            io.KeyAlt = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
            io.KeySuper = keyboard.IsKeyDown(Keys.LeftWindows) || keyboard.IsKeyDown(Keys.RightWindows);

            io.DisplaySize = new Num.Vector2(
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight
            );
            io.DisplayFramebufferScale = new Num.Vector2(1f, 1f);

            io.MousePos = new Num.Vector2(mouse.X, mouse.Y);

            io.MouseDown[0] = mouse.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouse.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = mouse.MiddleButton == ButtonState.Pressed;

            var scrollDelta = mouse.ScrollWheelValue - _scrollWheelValue;
            io.MouseWheel = scrollDelta > 0
                ? 1
                : scrollDelta < 0
                    ? -1
                    : 0;
            _scrollWheelValue = mouse.ScrollWheelValue;
        }

        private void RenderDrawData(ImDrawDataPtr drawData)
        {
            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers
            var lastViewport = _graphicsDevice.Viewport;
            var lastScissorBox = _graphicsDevice.ScissorRectangle;

            _graphicsDevice.BlendFactor = Color.White;
            _graphicsDevice.BlendState = BlendState.NonPremultiplied;
            _graphicsDevice.RasterizerState = _rasterizerState;
            _graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);

            // Setup projection
            _graphicsDevice.Viewport = new Viewport(
                0,
                0,
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight
            );

            UpdateBuffers(drawData);

            RenderCommandLists(drawData);

            // Restore modified state
            _graphicsDevice.Viewport = lastViewport;
            _graphicsDevice.ScissorRectangle = lastScissorBox;
        }

        private unsafe void UpdateBuffers(ImDrawDataPtr drawData)
        {
            if (drawData.TotalVtxCount == 0)
            {
                return;
            }

            if (drawData.TotalVtxCount > _vertexBufferSize)
            {
                _vertexBuffer?.Dispose();

                _vertexBufferSize = (int)(drawData.TotalVtxCount * 1.5f);
                _vertexBuffer = new VertexBuffer(
                    _graphicsDevice,
                    ImGuiDrawVertexDeclaration.Declaration,
                    _vertexBufferSize,
                    BufferUsage.None
                );
                _vertexData = new byte[_vertexBufferSize * ImGuiDrawVertexDeclaration.Size];
            }

            if (drawData.TotalIdxCount > _indexBufferSize)
            {
                _indexBuffer?.Dispose();

                _indexBufferSize = (int)(drawData.TotalIdxCount * 1.5f);
                _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, _indexBufferSize, BufferUsage.None);
                _indexData = new byte[_indexBufferSize * sizeof(ushort)];
            }

            // Copy ImGui's vertices and indices to a set of managed byte arrays
            var vtxOffset = 0;
            var idxOffset = 0;

            for (var n = 0; n < drawData.CmdListsCount; n++)
            {
                var cmdList = drawData.CmdListsRange[n];

                fixed (void* vtxDstPtr = &_vertexData[vtxOffset * ImGuiDrawVertexDeclaration.Size])
                {
                    fixed (void* idxDstPtr = &_indexData[idxOffset * sizeof(ushort)])
                    {
                        Buffer.MemoryCopy(
                            (void*)cmdList.VtxBuffer.Data,
                            vtxDstPtr,
                            _vertexData.Length,
                            cmdList.VtxBuffer.Size * ImGuiDrawVertexDeclaration.Size
                        );
                        Buffer.MemoryCopy(
                            (void*)cmdList.IdxBuffer.Data,
                            idxDstPtr,
                            _indexData.Length,
                            cmdList.IdxBuffer.Size * sizeof(ushort)
                        );
                    }
                }

                vtxOffset += cmdList.VtxBuffer.Size;
                idxOffset += cmdList.IdxBuffer.Size;
            }

            // Copy the managed byte arrays to the gpu vertex- and index buffers
            _vertexBuffer.SetData(_vertexData, 0, drawData.TotalVtxCount * ImGuiDrawVertexDeclaration.Size);
            _indexBuffer.SetData(_indexData, 0, drawData.TotalIdxCount * sizeof(ushort));
        }

        private unsafe void RenderCommandLists(ImDrawDataPtr drawData)
        {
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            var vtxOffset = 0;
            var idxOffset = 0;

            for (var n = 0; n < drawData.CmdListsCount; n++)
            {
                var cmdList = drawData.CmdListsRange[n];

                for (var cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
                {
                    var drawCmd = cmdList.CmdBuffer[cmdi];

                    if (!_loadedTextures.ContainsKey(drawCmd.TextureId))
                    {
                        throw new InvalidOperationException(
                            $"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings"
                        );
                    }

                    _graphicsDevice.ScissorRectangle = new Rectangle(
                        (int)drawCmd.ClipRect.X,
                        (int)drawCmd.ClipRect.Y,
                        (int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
                        (int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y)
                    );

                    var effect = UpdateEffect(_loadedTextures[drawCmd.TextureId]);

                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

#pragma warning disable CS0618 // // FNA does not expose an alternative method.
                        _graphicsDevice.DrawIndexedPrimitives(
                            primitiveType: PrimitiveType.TriangleList,
                            baseVertex: vtxOffset,
                            minVertexIndex: 0,
                            numVertices: cmdList.VtxBuffer.Size,
                            startIndex: idxOffset,
                            primitiveCount: (int)drawCmd.ElemCount / 3
                        );
#pragma warning restore CS0618
                    }

                    idxOffset += (int)drawCmd.ElemCount;
                }

                vtxOffset += cmdList.VtxBuffer.Size;
            }
        }

        private static void SetCherryStyle(ImGuiStylePtr style)
        {
            static Num.Vector4 Hi(float v) => new Num.Vector4(0.502f, 0.075f, 0.256f, v);
            static Num.Vector4 Medium(float v) => new Num.Vector4(0.455f, 0.198f, 0.301f, v);
            static Num.Vector4 Low(float v) => new Num.Vector4(0.232f, 0.201f, 0.271f, v);
            static Num.Vector4 Background(float v) => new Num.Vector4(0.200f, 0.220f, 0.270f, v);
            static Num.Vector4 Text(float v) => new Num.Vector4(0.860f, 0.930f, 0.890f, v);

            style.Colors[(int)ImGuiCol.Text] = Text(0.78f);
            style.Colors[(int)ImGuiCol.TextDisabled] = Text(0.28f);
            style.Colors[(int)ImGuiCol.WindowBg] = new Num.Vector4(0.13f, 0.14f, 0.17f, 1.00f);
            style.Colors[(int)ImGuiCol.ChildBg] = Background(0.58f);
            style.Colors[(int)ImGuiCol.PopupBg] = Background(0.9f);
            style.Colors[(int)ImGuiCol.Border] = new Num.Vector4(0.31f, 0.31f, 1.00f, 0.00f);
            style.Colors[(int)ImGuiCol.BorderShadow] = new Num.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            style.Colors[(int)ImGuiCol.FrameBg] = Background(1.00f);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = Medium(0.78f);
            style.Colors[(int)ImGuiCol.FrameBgActive] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.TitleBg] = Low(1.00f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = Hi(1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = Background(0.75f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = Background(0.47f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = Background(1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Num.Vector4(0.09f, 0.15f, 0.16f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = Medium(0.78f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.CheckMark] = new Num.Vector4(0.71f, 0.22f, 0.27f, 1.00f);
            style.Colors[(int)ImGuiCol.SliderGrab] = new Num.Vector4(0.47f, 0.77f, 0.83f, 0.14f);
            style.Colors[(int)ImGuiCol.SliderGrabActive] = new Num.Vector4(0.71f, 0.22f, 0.27f, 1.00f);
            style.Colors[(int)ImGuiCol.Button] = new Num.Vector4(0.47f, 0.77f, 0.83f, 0.14f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = Medium(0.86f);
            style.Colors[(int)ImGuiCol.ButtonActive] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.Header] = Medium(0.76f);
            style.Colors[(int)ImGuiCol.HeaderHovered] = Medium(0.86f);
            style.Colors[(int)ImGuiCol.HeaderActive] = Hi(1.00f);
            style.Colors[(int)ImGuiCol.ResizeGrip] = new Num.Vector4(0.47f, 0.77f, 0.83f, 0.04f);
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = Medium(0.78f);
            style.Colors[(int)ImGuiCol.ResizeGripActive] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.PlotLines] = Text(0.63f);
            style.Colors[(int)ImGuiCol.PlotLinesHovered] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogram] = Text(0.63f);
            style.Colors[(int)ImGuiCol.PlotHistogramHovered] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.TextSelectedBg] = Medium(0.43f);

            style.WindowPadding = new Num.Vector2(6, 4);
            style.WindowRounding = 0.0f;
            style.FramePadding = new Num.Vector2(5, 2);
            style.FrameRounding = 3.0f;
            style.ItemSpacing = new Num.Vector2(7, 1);
            style.ItemInnerSpacing = new Num.Vector2(1, 1);
            style.TouchExtraPadding = new Num.Vector2(0, 0);
            style.IndentSpacing = 6.0f;
            style.ScrollbarSize = 12.0f;
            style.ScrollbarRounding = 16.0f;
            style.GrabMinSize = 20.0f;
            style.GrabRounding = 2.0f;

            style.WindowTitleAlign.X = 0.50f;

            style.Colors[(int)ImGuiCol.Border] = new Num.Vector4(0.539f, 0.479f, 0.255f, 0.162f);
            style.FrameBorderSize = 0.0f;
            style.WindowBorderSize = 1.0f;
        }

        private static void SetDiscordStyle(ImGuiStylePtr style)
        {
            static Num.Vector4 Hi(float v) => new Num.Vector4(0.502f, 0.075f, 0.256f, v);
            static Num.Vector4 Medium(float v) => new Num.Vector4(0.2f, 0.2f, 0.301f, v);
            static Num.Vector4 Low(float v) => new Num.Vector4(0.232f, 0.201f, 0.271f, v);
            static Num.Vector4 Background(float v) => new Num.Vector4(54 / 255.0f, 57 / 255.0f, 63 / 255.0f, v);
            static Num.Vector4 Text(float v) => new Num.Vector4(0.860f, 0.930f, 0.890f, v);

            style.Colors[(int)ImGuiCol.WindowBg] = Background(1.00f);
            style.Colors[(int)ImGuiCol.ChildBg] = Background(0.58f);
            style.Colors[(int)ImGuiCol.PopupBg] = Background(0.9f);
            style.Colors[(int)ImGuiCol.FrameBg] = Background(1.00f);
            style.Colors[(int)ImGuiCol.TitleBg] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = Medium(0.75f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = Background(0.47f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = Background(1.00f);
        }

        private static void SetDiscordDarkerStyle(ImGuiStylePtr style)
        {
            static Num.Vector4 Hi(float v) => new Num.Vector4(0.502f, 0.075f, 0.256f, v);
            static Num.Vector4 Medium(float v) => new Num.Vector4(0.455f, 0.198f, 0.301f, v);
            static Num.Vector4 Low(float v) => new Num.Vector4(0.232f, 0.201f, 0.271f, v);
            static Num.Vector4 Background(float v) => new Num.Vector4(47 / 255.0f, 49 / 255.0f, 54 / 255.0f, v);
            static Num.Vector4 Text(float v) => new Num.Vector4(0.860f, 0.930f, 0.890f, v);

            style.Colors[(int)ImGuiCol.WindowBg] = Background(1.00f);
            style.Colors[(int)ImGuiCol.ChildBg] = Background(0.58f);
            style.Colors[(int)ImGuiCol.PopupBg] = Background(0.9f);
            style.Colors[(int)ImGuiCol.FrameBg] = Background(1.00f);
            style.Colors[(int)ImGuiCol.TitleBg] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = Medium(0.75f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = Background(0.47f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = Background(1.00f);
        }

        private static void SetDiscordDarkStyle(ImGuiStylePtr style)
        {
            static Num.Vector4 Hi(float v) => new Num.Vector4(0.502f, 0.075f, 0.256f, v);
            static Num.Vector4 Medium(float v) => new Num.Vector4(0.455f, 0.198f, 0.301f, v);
            static Num.Vector4 Low(float v) => new Num.Vector4(0.232f, 0.201f, 0.271f, v);
            static Num.Vector4 Background(float v) => new Num.Vector4(32 / 255.0f, 34 / 255.0f, 37 / 255.0f, v);
            static Num.Vector4 Text(float v) => new Num.Vector4(0.860f, 0.930f, 0.890f, v);

            style.Colors[(int)ImGuiCol.WindowBg] = Background(1.00f);
            style.Colors[(int)ImGuiCol.ChildBg] = Background(0.58f);
            style.Colors[(int)ImGuiCol.PopupBg] = Background(0.9f);
            style.Colors[(int)ImGuiCol.FrameBg] = Background(1.00f);
            style.Colors[(int)ImGuiCol.TitleBg] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = Medium(0.75f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = Background(0.47f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = Background(1.00f);
        }

        private static void SetRedStyle(ImGuiStylePtr style)
        {
            style.Colors[(int)ImGuiCol.Text] = new Num.Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TextDisabled] = new Num.Vector4(0.73f, 0.75f, 0.74f, 1.00f);
            style.Colors[(int)ImGuiCol.WindowBg] = new Num.Vector4(0.09f, 0.09f, 0.09f, 0.94f);
            style.Colors[(int)ImGuiCol.ChildBg] = new Num.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            style.Colors[(int)ImGuiCol.PopupBg] = new Num.Vector4(0.08f, 0.08f, 0.08f, 0.94f);
            style.Colors[(int)ImGuiCol.Border] = new Num.Vector4(0.20f, 0.20f, 0.20f, 0.50f);
            style.Colors[(int)ImGuiCol.BorderShadow] = new Num.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            style.Colors[(int)ImGuiCol.FrameBg] = new Num.Vector4(0.71f, 0.39f, 0.39f, 0.54f);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = new Num.Vector4(0.84f, 0.66f, 0.66f, 0.40f);
            style.Colors[(int)ImGuiCol.FrameBgActive] = new Num.Vector4(0.84f, 0.66f, 0.66f, 0.67f);
            style.Colors[(int)ImGuiCol.TitleBg] = new Num.Vector4(0.47f, 0.22f, 0.22f, 0.67f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new Num.Vector4(0.47f, 0.22f, 0.22f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Num.Vector4(0.47f, 0.22f, 0.22f, 0.67f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = new Num.Vector4(0.34f, 0.16f, 0.16f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = new Num.Vector4(0.02f, 0.02f, 0.02f, 0.53f);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Num.Vector4(0.31f, 0.31f, 0.31f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Num.Vector4(0.41f, 0.41f, 0.41f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Num.Vector4(0.51f, 0.51f, 0.51f, 1.00f);
            style.Colors[(int)ImGuiCol.CheckMark] = new Num.Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.SliderGrab] = new Num.Vector4(0.71f, 0.39f, 0.39f, 1.00f);
            style.Colors[(int)ImGuiCol.SliderGrabActive] = new Num.Vector4(0.84f, 0.66f, 0.66f, 1.00f);
            style.Colors[(int)ImGuiCol.Button] = new Num.Vector4(0.47f, 0.22f, 0.22f, 0.65f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new Num.Vector4(0.71f, 0.39f, 0.39f, 0.65f);
            style.Colors[(int)ImGuiCol.ButtonActive] = new Num.Vector4(0.20f, 0.20f, 0.20f, 0.50f);
            style.Colors[(int)ImGuiCol.Header] = new Num.Vector4(0.71f, 0.39f, 0.39f, 0.54f);
            style.Colors[(int)ImGuiCol.HeaderHovered] = new Num.Vector4(0.84f, 0.66f, 0.66f, 0.65f);
            style.Colors[(int)ImGuiCol.HeaderActive] = new Num.Vector4(0.84f, 0.66f, 0.66f, 0.00f);
            style.Colors[(int)ImGuiCol.Separator] = new Num.Vector4(0.43f, 0.43f, 0.50f, 0.50f);
            style.Colors[(int)ImGuiCol.SeparatorHovered] = new Num.Vector4(0.71f, 0.39f, 0.39f, 0.54f);
            style.Colors[(int)ImGuiCol.SeparatorActive] = new Num.Vector4(0.71f, 0.39f, 0.39f, 0.54f);
            style.Colors[(int)ImGuiCol.ResizeGrip] = new Num.Vector4(0.71f, 0.39f, 0.39f, 0.54f);
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Num.Vector4(0.84f, 0.66f, 0.66f, 0.66f);
            style.Colors[(int)ImGuiCol.ResizeGripActive] = new Num.Vector4(0.84f, 0.66f, 0.66f, 0.66f);
            style.Colors[(int)ImGuiCol.Tab] = new Num.Vector4(0.71f, 0.39f, 0.39f, 0.54f);
            style.Colors[(int)ImGuiCol.TabHovered] = new Num.Vector4(0.84f, 0.66f, 0.66f, 0.66f);
            style.Colors[(int)ImGuiCol.TabActive] = new Num.Vector4(0.84f, 0.66f, 0.66f, 0.66f);
            style.Colors[(int)ImGuiCol.TabUnfocused] = new Num.Vector4(0.07f, 0.10f, 0.15f, 0.97f);
            style.Colors[(int)ImGuiCol.TabUnfocusedActive] = new Num.Vector4(0.14f, 0.26f, 0.42f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLines] = new Num.Vector4(0.61f, 0.61f, 0.61f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Num.Vector4(1.00f, 0.43f, 0.35f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogram] = new Num.Vector4(0.90f, 0.70f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Num.Vector4(1.00f, 0.60f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TextSelectedBg] = new Num.Vector4(0.26f, 0.59f, 0.98f, 0.35f);
            style.Colors[(int)ImGuiCol.DragDropTarget] = new Num.Vector4(1.00f, 1.00f, 0.00f, 0.90f);
            style.Colors[(int)ImGuiCol.NavHighlight] = new Num.Vector4(0.41f, 0.41f, 0.41f, 1.00f);
            style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new Num.Vector4(1.00f, 1.00f, 1.00f, 0.70f);
            style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new Num.Vector4(0.80f, 0.80f, 0.80f, 0.20f);
            style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Num.Vector4(0.80f, 0.80f, 0.80f, 0.35f);
        }

        private static void SetXStyle(ImGuiStylePtr style)
        {
            var colors = ImGui.GetStyle().Colors;
            colors[(int)ImGuiCol.Text] = new Num.Vector4(0.92f, 0.63f, 0.13f, 1.00f);
            colors[(int)ImGuiCol.TextDisabled] = new Num.Vector4(0.39f, 0.29f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.WindowBg] = new Num.Vector4(0.00f, 0.00f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.ChildBg] = new Num.Vector4(0.10f, 0.11f, 0.12f, 1.00f);
            colors[(int)ImGuiCol.PopupBg] = new Num.Vector4(0.21f, 0.22f, 0.25f, 0.90f);
            colors[(int)ImGuiCol.Border] = new Num.Vector4(0.43f, 0.43f, 0.50f, 0.50f);
            colors[(int)ImGuiCol.BorderShadow] = new Num.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            colors[(int)ImGuiCol.FrameBg] = new Num.Vector4(0.04f, 0.04f, 0.04f, 1.00f);
            colors[(int)ImGuiCol.FrameBgHovered] = new Num.Vector4(0.98f, 0.59f, 0.01f, 0.40f);
            colors[(int)ImGuiCol.FrameBgActive] = new Num.Vector4(0.89f, 0.64f, 0.02f, 0.67f);
            colors[(int)ImGuiCol.TitleBg] = new Num.Vector4(0.27f, 0.27f, 0.27f, 1.00f);
            colors[(int)ImGuiCol.TitleBgActive] = new Num.Vector4(0.22f, 0.22f, 0.22f, 1.00f);
            colors[(int)ImGuiCol.TitleBgCollapsed] = new Num.Vector4(0.20f, 0.20f, 0.30f, 0.75f);
            colors[(int)ImGuiCol.MenuBarBg] = new Num.Vector4(0.25f, 0.25f, 0.25f, 0.47f);
            colors[(int)ImGuiCol.ScrollbarBg] = new Num.Vector4(0.21f, 0.22f, 0.25f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrab] = new Num.Vector4(0.31f, 0.31f, 0.31f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Num.Vector4(0.41f, 0.41f, 0.41f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabActive] = new Num.Vector4(0.51f, 0.51f, 0.51f, 1.00f);
            colors[(int)ImGuiCol.CheckMark] = new Num.Vector4(1.00f, 0.75f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.SliderGrab] = new Num.Vector4(1.00f, 0.67f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.SliderGrabActive] = new Num.Vector4(0.94f, 0.36f, 0.07f, 1.00f);
            colors[(int)ImGuiCol.Button] = new Num.Vector4(0.53f, 0.53f, 0.53f, 0.40f);
            colors[(int)ImGuiCol.ButtonHovered] = new Num.Vector4(0.44f, 0.48f, 0.44f, 1.00f);
            colors[(int)ImGuiCol.ButtonActive] = new Num.Vector4(0.85f, 0.85f, 0.85f, 1.00f);
            colors[(int)ImGuiCol.Header] = new Num.Vector4(0.81f, 0.55f, 0.03f, 0.31f);
            colors[(int)ImGuiCol.HeaderHovered] = new Num.Vector4(0.89f, 0.45f, 0.03f, 0.80f);
            colors[(int)ImGuiCol.HeaderActive] = new Num.Vector4(0.92f, 0.33f, 0.03f, 1.00f);
            colors[(int)ImGuiCol.Separator] = new Num.Vector4(0.43f, 0.43f, 0.50f, 0.50f);
            colors[(int)ImGuiCol.SeparatorHovered] = new Num.Vector4(0.93f, 0.29f, 0.02f, 0.78f);
            colors[(int)ImGuiCol.SeparatorActive] = new Num.Vector4(0.90f, 0.21f, 0.05f, 1.00f);
            colors[(int)ImGuiCol.ResizeGrip] = new Num.Vector4(1.00f, 0.82f, 0.00f, 0.25f);
            colors[(int)ImGuiCol.ResizeGripHovered] = new Num.Vector4(0.93f, 0.31f, 0.05f, 0.67f);
            colors[(int)ImGuiCol.ResizeGripActive] = new Num.Vector4(0.93f, 0.38f, 0.06f, 0.95f);
            colors[(int)ImGuiCol.Tab] = new Num.Vector4(0.27f, 0.27f, 0.27f, 0.86f);
            colors[(int)ImGuiCol.TabHovered] = new Num.Vector4(0.52f, 0.52f, 0.52f, 0.80f);
            colors[(int)ImGuiCol.TabActive] = new Num.Vector4(0.49f, 0.49f, 0.49f, 0.90f);
            colors[(int)ImGuiCol.TabUnfocused] = new Num.Vector4(0.22f, 0.22f, 0.22f, 0.97f);
            colors[(int)ImGuiCol.TabUnfocusedActive] = new Num.Vector4(0.16f, 0.16f, 0.16f, 1.00f);
            colors[(int)ImGuiCol.DockingPreview] = new Num.Vector4(0.88f, 0.14f, 0.01f, 0.70f);
            colors[(int)ImGuiCol.DockingEmptyBg] = new Num.Vector4(0.14f, 0.14f, 0.14f, 1.00f);
            colors[(int)ImGuiCol.PlotLines] = new Num.Vector4(0.61f, 0.61f, 0.61f, 1.00f);
            colors[(int)ImGuiCol.PlotLinesHovered] = new Num.Vector4(1.00f, 0.43f, 0.35f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogram] = new Num.Vector4(0.90f, 0.70f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogramHovered] = new Num.Vector4(1.00f, 0.60f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.TextSelectedBg] = new Num.Vector4(0.80f, 0.48f, 0.02f, 0.35f);
            colors[(int)ImGuiCol.DragDropTarget] = new Num.Vector4(1.00f, 1.00f, 0.00f, 0.90f);
            colors[(int)ImGuiCol.NavHighlight] = new Num.Vector4(0.90f, 0.68f, 0.04f, 1.00f);
            colors[(int)ImGuiCol.NavWindowingHighlight] = new Num.Vector4(1.00f, 1.00f, 1.00f, 0.70f);
            colors[(int)ImGuiCol.NavWindowingDimBg] = new Num.Vector4(0.80f, 0.80f, 0.80f, 0.20f);
            colors[(int)ImGuiCol.ModalWindowDimBg] = new Num.Vector4(0.80f, 0.80f, 0.80f, 0.35f);
        }

        public static void SetStyle(UiStyle uiStyle)
        {
            var style = ImGui.GetStyle();
            switch (uiStyle)
            {
                case UiStyle.Dark:
                    ImGui.StyleColorsDark();
                    break;
                case UiStyle.Light:
                    ImGui.StyleColorsLight();
                    break;
                case UiStyle.Discord:
                    ImGui.StyleColorsDark();
                    SetDiscordStyle(style);
                    break;
                case UiStyle.DiscordDarker:
                    ImGui.StyleColorsDark();
                    SetDiscordDarkerStyle(style);
                    break;
                case UiStyle.DiscordDark:
                    ImGui.StyleColorsDark();
                    SetDiscordDarkStyle(style);
                    break;
                case UiStyle.Cherry:
                    SetCherryStyle(style);
                    break;
                case UiStyle.Red:
                    SetXStyle(style);
                    break;
            }
        }
    }
}
