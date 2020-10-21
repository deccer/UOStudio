using System;
using System.Runtime.InteropServices;
using ImGuiNET;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Num = System.Numerics;

namespace UOStudio.Client.Engine.UI
{
    public sealed class ImGuiRenderer
    {
        private readonly ImGuiIndexData _imGuiIndex;
        private readonly ImGuiVertexData _imGuiVertex;
        private readonly ImGuiInputHandler _imGuiInputHandler;
        private readonly ImGuiTextureData _imGuiTextureData;
        private readonly BasicEffect _effect;
        private readonly RasterizerState _rasterizerState;

        public ImGuiRenderer([NotNull] Game game, [NotNull] ImGuiInputHandler inputHandler)
        {
            Game = game ?? throw new ArgumentNullException(nameof(game));
            _imGuiInputHandler = inputHandler ?? throw new ArgumentNullException(nameof(inputHandler));

            _effect = new BasicEffect(game.GraphicsDevice);
            _imGuiTextureData = new ImGuiTextureData();
            _rasterizerState = RasterizerState.CullNone;
            _imGuiVertex = new ImGuiVertexData();
            _imGuiIndex = new ImGuiIndexData();
        }

        public Game Game { get; }

        public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

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

        public IntPtr BindTexture(Texture2D texture)
        {
            var id = new IntPtr(_imGuiTextureData.GetTextureId());
            _imGuiTextureData.Loaded.Add(id, texture);

            return id;
        }

        public void UnbindTexture(IntPtr textureId)
        {
            _imGuiTextureData.Loaded.Remove(textureId);
        }

        public ImGuiRenderer Initialize()
        {
            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            var style = ImGui.GetStyle();
            SetCherryStyle(style);

            _imGuiInputHandler.Initialize(Game);
            return this;
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
            //style.Colors[(int)ImGuiCol.Column]                = new Vector4(0.14f, 0.16f, 0.19f, 1.00f);
            //style.Colors[(int)ImGuiCol.ColumnHovered]         = Medium( 0.78f);
            //style.Colors[(int)ImGuiCol.ColumnActive]          = Medium( 1.00f);
            style.Colors[(int)ImGuiCol.ResizeGrip] = new Num.Vector4(0.47f, 0.77f, 0.83f, 0.04f);
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = Medium(0.78f);
            style.Colors[(int)ImGuiCol.ResizeGripActive] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.PlotLines] = Text(0.63f);
            style.Colors[(int)ImGuiCol.PlotLinesHovered] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogram] = Text(0.63f);
            style.Colors[(int)ImGuiCol.PlotHistogramHovered] = Medium(1.00f);
            style.Colors[(int)ImGuiCol.TextSelectedBg] = Medium(0.43f);
            //style.Colors[ImGuiCol.ModalWindowDarkening]  = Background( 0.73f);

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

        public unsafe ImGuiRenderer RebuildFontAtlas()
        {
            var io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out int bytesPerPixel);

            var pixels = new byte[width * height * bytesPerPixel];
            Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length);

            var texture = new Texture2D(GraphicsDevice, width, height, false, SurfaceFormat.Color);
            texture.SetData(pixels);

            if (_imGuiTextureData.FontTextureId.HasValue)
            {
                UnbindTexture(_imGuiTextureData.FontTextureId.Value);
            }

            _imGuiTextureData.FontTextureId = BindTexture(texture);

            io.Fonts.SetTexID(_imGuiTextureData.FontTextureId.Value);
            io.Fonts.ClearTexData();
            return this;
        }

        private void RenderDrawData(ImDrawDataPtr drawData)
        {
            var lastViewport = GraphicsDevice.Viewport;
            var lastScissorRect = GraphicsDevice.ScissorRectangle;

            GraphicsDevice.BlendFactor = Color.White;
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            GraphicsDevice.RasterizerState = _rasterizerState;
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);

            GraphicsDevice.Viewport = new Viewport(
                0,
                0,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight
            );

            UpdateBuffers(drawData);
            RenderCommandLists(drawData);

            GraphicsDevice.Viewport = lastViewport;
            GraphicsDevice.ScissorRectangle = lastScissorRect;
        }

        private void RenderCommandLists(ImDrawDataPtr draw_data)
        {
            GraphicsDevice.SetVertexBuffer(_imGuiVertex.Buffer);
            GraphicsDevice.Indices = _imGuiIndex.Buffer;

            var vertexOffset = 0;
            var indexOffset = 0;
            for (var i = 0; i < draw_data.CmdListsCount; ++i)
            {
                var commandList = draw_data.CmdListsRange[i];
                for (var commandIndex = 0; commandIndex < commandList.CmdBuffer.Size; ++commandIndex)
                {
                    var drawCommand = commandList.CmdBuffer[commandIndex];

                    if (!_imGuiTextureData.Loaded.ContainsKey(drawCommand.TextureId))
                    {
                        throw new ImGuiMissingLoadedTextureKeyException(drawCommand.TextureId);
                    }

                    GraphicsDevice.ScissorRectangle = GenerateScissorRect(drawCommand);
                    var effect = UpdateEffect(_imGuiTextureData.Loaded[drawCommand.TextureId]);

                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        DrawPrimitives(vertexOffset, indexOffset, commandList, drawCommand);
                    }

                    indexOffset += (int)drawCommand.ElemCount;
                }

                vertexOffset += commandList.VtxBuffer.Size;
            }
        }

        private static Rectangle GenerateScissorRect(ImDrawCmdPtr drawCommand) =>
            new Rectangle(
                (int)drawCommand.ClipRect.X,
                (int)drawCommand.ClipRect.Y,
                (int)(drawCommand.ClipRect.Z - drawCommand.ClipRect.X),
                (int)(drawCommand.ClipRect.W - drawCommand.ClipRect.Y)
            );

        private void DrawPrimitives(int vertexOffset, int indexOffset, ImDrawListPtr commandList, ImDrawCmdPtr drawCommand)
        {
            GraphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                vertexOffset,
                0,
                commandList.VtxBuffer.Size,
                indexOffset,
                (int)(drawCommand.ElemCount / 3)
            );
        }

        private Effect UpdateEffect(Texture2D texture)
        {
            var io = ImGui.GetIO();
            var displaySize = io.DisplaySize;

            const float offset = 0f;
            _effect.World = Matrix.Identity;
            _effect.View = Matrix.Identity;
            _effect.Projection = Matrix.CreateOrthographicOffCenter(
                offset,
                displaySize.X + offset,
                displaySize.Y + offset,
                offset,
                -1.0f,
                1.0f
            );
            _effect.TextureEnabled = true;
            _effect.Texture = texture;
            _effect.VertexColorEnabled = true;

            return _effect;
        }

        private unsafe void UpdateBuffers(ImDrawDataPtr drawData)
        {
            if (drawData.TotalVtxCount == 0)
            {
                return;
            }

            if (drawData.TotalVtxCount > _imGuiVertex.BufferSize)
            {
                _imGuiVertex.Buffer?.Dispose();
                _imGuiVertex.BufferSize = (int)(drawData.TotalVtxCount * 1.5f);
                _imGuiVertex.Buffer = new VertexBuffer(
                    GraphicsDevice,
                    DrawVertexDeclaration.Declaration,
                    _imGuiVertex.BufferSize,
                    BufferUsage.None
                );
                _imGuiVertex.Data = new byte[_imGuiVertex.BufferSize * DrawVertexDeclaration.Size];
            }

            if (drawData.TotalIdxCount > _imGuiIndex.BufferSize)
            {
                _imGuiIndex.Buffer?.Dispose();

                _imGuiIndex.BufferSize = (int)(drawData.TotalIdxCount * 1.5f);
                _imGuiIndex.Buffer = new IndexBuffer(
                    GraphicsDevice,
                    IndexElementSize.SixteenBits,
                    _imGuiIndex.BufferSize,
                    BufferUsage.None
                );
                _imGuiIndex.Data = new byte[_imGuiIndex.BufferSize * sizeof(ushort)];
            }

            var vertexOffset = 0;
            var indexOffset = 0;

            for (var i = 0; i < drawData.CmdListsCount; ++i)
            {
                var commands = drawData.CmdListsRange[i];
                fixed (void* vtxDstPtr = &_imGuiVertex.Data[vertexOffset * DrawVertexDeclaration.Size])
                {
                    fixed (void* idxDstPtr = &_imGuiIndex.Data[indexOffset * sizeof(ushort)])
                    {
                        Buffer.MemoryCopy(
                            (void*)commands.VtxBuffer.Data,
                            vtxDstPtr,
                            _imGuiVertex.Data.Length,
                            commands.VtxBuffer.Size * DrawVertexDeclaration.Size
                        );
                        Buffer.MemoryCopy(
                            (void*)commands.IdxBuffer.Data,
                            idxDstPtr,
                            _imGuiIndex.Data.Length,
                            commands.IdxBuffer.Size * sizeof(ushort)
                        );
                    }
                }

                vertexOffset += commands.VtxBuffer.Size;
                indexOffset += commands.IdxBuffer.Size;
            }

            _imGuiVertex.Buffer.SetData(_imGuiVertex.Data, 0, drawData.TotalVtxCount * DrawVertexDeclaration.Size);
            _imGuiIndex.Buffer.SetData(_imGuiIndex.Data, 0, drawData.TotalIdxCount * sizeof(ushort));
        }
    }
}
