using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Num = System.Numerics;

namespace UOStudio.Client.UI
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

            io.Fonts.AddFontFromFileTTF("Content/Fonts/Ruda-Regular.ttf", 20);

            var style = ImGui.GetStyle();

            SetStyleDarker(style);

            style.Colors[(int)ImGuiCol.DockingEmptyBg] = Num.Vector4.Zero;
            SetupInput(io);
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
            ImGui.NewFrame();
        }

        public void EndLayout()
        {
            ImGui.Render();

            RenderDrawData(ImGui.GetDrawData());
        }

        [SuppressMessage("ReSharper", "S1121")]
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

        private void RenderCommandLists(ImDrawDataPtr drawData)
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

        private void SetBlackGrayRedStyle(ImGuiStylePtr style)
        {
            var chk = new Num.Vector4(0.6f, 0.0f, 0.0f, 1.00f);
            var ch1 = new Num.Vector4(0.4f, 0.0f, 0.0f, 1.00f);
            var ch2 = new Num.Vector4(0.5f, 0.0f, 0.0f, 1.00f);
            var but = new Num.Vector4(0.40f, 0.50f, 0.75f, 1.00f);

            style.Colors[(int)ImGuiCol.Text] = new Num.Vector4(0.90f, 0.90f, 0.90f, 0.90f);
            style.Colors[(int)ImGuiCol.TextDisabled] = new Num.Vector4(0.60f, 0.60f, 0.60f, 1.00f);
            style.Colors[(int)ImGuiCol.WindowBg] = new Num.Vector4(0.09f, 0.09f, 0.15f, 1.00f);
            style.Colors[(int)ImGuiCol.ChildBg] = new Num.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            style.Colors[(int)ImGuiCol.PopupBg] = new Num.Vector4(0.00f, 0.00f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.Border] = new Num.Vector4(0.70f, 0.70f, 0.70f, 0.65f);
            style.Colors[(int)ImGuiCol.BorderShadow] = new Num.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            style.Colors[(int)ImGuiCol.FrameBg] = new Num.Vector4(0.00f, 0.00f, 0.01f, 1.00f);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = new Num.Vector4(0.70f, 0.70f, 0.70f, 0.20f);
            style.Colors[(int)ImGuiCol.FrameBgActive] = new Num.Vector4(0.90f, 0.00f, 0.00f, 0.75f);
            style.Colors[(int)ImGuiCol.TitleBg] = new Num.Vector4(0.00f, 0.00f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Num.Vector4(but.X, but.Y, but.Z, 0.35f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new Num.Vector4(0.30f, 0.30f, 0.37f, 0.70f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = new Num.Vector4(0.01f, 0.01f, 0.02f, 0.80f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = new Num.Vector4(0.20f, 0.25f, 0.30f, 0.60f);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Num.Vector4(0.55f, 0.53f, 0.55f, 0.51f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = ch1;
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = ch2;
            style.Colors[(int)ImGuiCol.CheckMark] = new Num.Vector4(chk.X, chk.Y, chk.Z, 1.00f);
            style.Colors[(int)ImGuiCol.SliderGrab] = ch1;
            style.Colors[(int)ImGuiCol.SliderGrabActive] = ch2;
            style.Colors[(int)ImGuiCol.Button] = new Num.Vector4(0.50f, 0.50f, 0.50f, 0.50f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new Num.Vector4(but.X, but.Y, but.Z, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonActive] = new Num.Vector4(0.60f, 0.00f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.Header] = new Num.Vector4(but.X, but.Y, but.Z, 0.30f);
            style.Colors[(int)ImGuiCol.HeaderHovered] = new Num.Vector4(but.X, but.Y, but.Z, 0.75f);
            style.Colors[(int)ImGuiCol.HeaderActive] = new Num.Vector4(but.X, but.Y, but.Z, 0.50f);
            style.Colors[(int)ImGuiCol.Separator] = new Num.Vector4(0.50f, 0.50f, 0.50f, 1.00f);
            style.Colors[(int)ImGuiCol.SeparatorHovered] = new Num.Vector4(0.70f, 0.60f, 0.60f, 1.00f);
            style.Colors[(int)ImGuiCol.SeparatorActive] = new Num.Vector4(0.90f, 0.70f, 0.70f, 1.00f);
            style.Colors[(int)ImGuiCol.ResizeGrip] = new Num.Vector4(0.50f, 0.50f, 0.50f, 0.50f);
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = ch1;
            style.Colors[(int)ImGuiCol.ResizeGripActive] = ch2;
            style.Colors[(int)ImGuiCol.PlotLines] = new Num.Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Num.Vector4(0.90f, 0.70f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogram] = ch1;
            style.Colors[(int)ImGuiCol.PlotHistogramHovered] = ch2;
            style.Colors[(int)ImGuiCol.TextSelectedBg] = new Num.Vector4(0.00f, 0.00f, 1.00f, 0.35f);
            style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Num.Vector4(0.20f, 0.20f, 0.20f, 0.35f);
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

        private void SetStyle2(ImGuiStylePtr style)
        {
            var color_for_text = new Num.Vector3(236f / 255f, 240f / 255f, 241f / 255f);
            var color_for_head = new Num.Vector3(41f / 255f, 128f / 255f, 185f / 255f);
            var color_for_area = new Num.Vector3(57f / 255f, 79f / 255f, 105f / 255f);
            var color_for_body = new Num.Vector3(44f / 255f, 62f / 255f, 80f / 255f);
            var color_for_pops = new Num.Vector3(33f / 255f, 46f / 255f, 60f / 255f);

            EasyTheming(style, color_for_text, color_for_head, color_for_area, color_for_body);
        }

        public void EasyTheming(ImGuiStylePtr style, Num.Vector3 color_for_text, Num.Vector3 color_for_head, Num.Vector3 color_for_area, Num.Vector3 color_for_body)
        {
            style.Colors[(int)ImGuiCol.Text] = new Num.Vector4(color_for_text.X, color_for_text.Y, color_for_text.Z, 1.00f);
            style.Colors[(int)ImGuiCol.TextDisabled] = new Num.Vector4(color_for_text.X, color_for_text.Y, color_for_text.Z, 0.58f);
            style.Colors[(int)ImGuiCol.WindowBg] = new Num.Vector4(color_for_body.X, color_for_body.Y, color_for_body.Z, 1.00f);
            style.Colors[(int)ImGuiCol.ChildBg] = new Num.Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 0.58f);
            style.Colors[(int)ImGuiCol.PopupBg] = new Num.Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 1.00f);
            style.Colors[(int)ImGuiCol.Border] = new Num.Vector4(color_for_body.X, color_for_body.Y, color_for_body.Z, 0.00f);
            style.Colors[(int)ImGuiCol.BorderShadow] = new Num.Vector4(color_for_body.X, color_for_body.Y, color_for_body.Z, 0.00f);
            style.Colors[(int)ImGuiCol.FrameBg] = new Num.Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 1.00f);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.78f);
            style.Colors[(int)ImGuiCol.FrameBgActive] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBg] = new Num.Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Num.Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 0.75f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = new Num.Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = new Num.Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.50f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.78f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
            style.Colors[(int)ImGuiCol.CheckMark] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
            style.Colors[(int)ImGuiCol.SliderGrab] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.50f);
            style.Colors[(int)ImGuiCol.SliderGrabActive] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
            style.Colors[(int)ImGuiCol.Button] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.50f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.86f);
            style.Colors[(int)ImGuiCol.ButtonActive] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
            style.Colors[(int)ImGuiCol.Header] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.76f);
            style.Colors[(int)ImGuiCol.HeaderHovered] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.86f);
            style.Colors[(int)ImGuiCol.HeaderActive] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
            style.Colors[(int)ImGuiCol.Separator] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.32f);
            style.Colors[(int)ImGuiCol.SeparatorHovered] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.78f);
            style.Colors[(int)ImGuiCol.SeparatorActive] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
            style.Colors[(int)ImGuiCol.ResizeGrip] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.15f);
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.78f);
            style.Colors[(int)ImGuiCol.ResizeGripActive] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLines] = new Num.Vector4(color_for_text.X, color_for_text.Y, color_for_text.Z, 0.63f);
            style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogram] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.50f);
            style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.75f);
            style.Colors[(int)ImGuiCol.TextSelectedBg] = new Num.Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.43f);
            style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Num.Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 0.73f);
        }

        private static class ImGuiDrawVertexDeclaration
        {
            public static readonly int Size;
            public static readonly VertexDeclaration Declaration;

            static ImGuiDrawVertexDeclaration()
            {
                unsafe { Size = sizeof(ImDrawVert); }

                var position = new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0);
                var uv = new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
                var color = new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0);
                Declaration = new VertexDeclaration(Size, position, uv, color);
            }
        }
    }
}
