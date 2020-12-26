using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UOStudio.Client.Engine
{
    public sealed unsafe class TileBatcher : IDisposable
    {
        private const int MaxSprites = 0x800;
        private const int MaxVertices = MaxSprites * 4;
        private const int MaxIndices = MaxSprites * 6;

        private int _numSprites;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly IndexBuffer _indexBuffer;
        private readonly DynamicVertexBuffer _vertexBuffer;
        private VertexPositionNormalTextureColor4* _vertices;
        private readonly Texture2D[] _textures;
        private readonly IsometricEffect _effect;
        private Effect _customEffect;
        private bool _isStarted;
        private int _currentBufferPosition;
        private Matrix _transformMatrix;
        private Matrix _projectionMatrix;

        private BlendState _blendState;
        private RasterizerState _rasterizerState;
        private SamplerState _sampler;
        private DepthStencilState _stencilState;

        public TileBatcher(GraphicsDevice graphicsDevice, Effect batcherEffect)
        {
            _graphicsDevice = graphicsDevice;
            _textures = new Texture2D[MaxSprites];
            _vertices = (VertexPositionNormalTextureColor4*)Marshal.AllocHGlobal(
                VertexPositionNormalTextureColor4.SizeInBytes * MaxSprites * 4);
            _vertexBuffer = new DynamicVertexBuffer(
                graphicsDevice,
                typeof(VertexPositionNormalTextureColor4),
                MaxSprites,
                BufferUsage.WriteOnly
            );
            _vertexBuffer.Name = "TileBatcher.VB";
            _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, MaxIndices, BufferUsage.WriteOnly);
            _indexBuffer.SetData(GenerateIndices());
            _indexBuffer.Name = "TileBatcher.IB";
            _projectionMatrix = new Matrix
            (
                0f,                         //(float)( 2.0 / (double)viewport.Width ) is the actual value we will use
                0.0f, 0.0f, 0.0f, 0.0f, 0f, //(float)( -2.0 / (double)viewport.Height ) is the actual value we will use
                0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, -1.0f, 1.0f, 0.0f, 1.0f
            );

            _projectionMatrix = Matrix.CreateOrthographic(800, 450, 0.1f, 2048f);

            _effect = new IsometricEffect(graphicsDevice, batcherEffect);

            _blendState = BlendState.AlphaBlend;
            _rasterizerState = RasterizerState.CullNone;
            _sampler = SamplerState.PointClamp;
            _rasterizerState = new RasterizerState
            {
                CullMode = _rasterizerState.CullMode,
                DepthBias = _rasterizerState.DepthBias,
                FillMode = _rasterizerState.FillMode,
                MultiSampleAntiAlias = _rasterizerState.MultiSampleAntiAlias,
                SlopeScaleDepthBias = _rasterizerState.SlopeScaleDepthBias,
                ScissorTestEnable = true
            };
            _stencilState = new DepthStencilState()
            {
                StencilEnable = false,
                DepthBufferEnable = false,
                StencilFunction = CompareFunction.NotEqual,
                ReferenceStencil = 1,
                StencilMask = 1,
                StencilFail = StencilOperation.Keep,
                StencilDepthBufferFail = StencilOperation.Keep,
                StencilPass = StencilOperation.Keep
            };
        }

        public void Begin()
        {
            Begin(null, Matrix.Identity);
        }

        public void Begin(Effect customEffect, Matrix transformMatrix)
        {
            EnsureNotStarted();
            _isStarted = true;

            _customEffect = customEffect;
            _transformMatrix = transformMatrix;
        }

        public void Dispose()
        {
            if (_vertices != null)
            {
                Marshal.FreeHGlobal((IntPtr)_vertices);
                _vertices = null;
            }

            _effect?.Dispose();
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }

        public bool DrawTile
        (
            Texture2D texture,
            int x,
            int y,
            ref Rectangle rect,
            ref Vector3 normal0,
            ref Vector3 normal1,
            ref Vector3 normal2,
            ref Vector3 normal3,
            ref Vector3 hue
        )
        {
            EnsureSize();

            ref var vertex = ref _vertices[_numSprites];

            vertex.TextureCoordinate0.X = 0;
            vertex.TextureCoordinate0.Y = 0;
            vertex.TextureCoordinate0.Z = 0;

            vertex.TextureCoordinate1.X = 1;
            vertex.TextureCoordinate1.Y = vertex.TextureCoordinate1.Z = 0;

            vertex.TextureCoordinate2.X = vertex.TextureCoordinate2.Z = 0;
            vertex.TextureCoordinate2.Y = 1;

            vertex.TextureCoordinate3.X = vertex.TextureCoordinate3.Y = 1;
            vertex.TextureCoordinate3.Z = 0;

            vertex.Normal0 = normal0;
            vertex.Normal1 = normal1;
            vertex.Normal3 = normal2; // right order!
            vertex.Normal2 = normal3;

            vertex.Position0.X = x + Tile.TileSizeHalf;
            vertex.Position0.Y = y - rect.Left;
            vertex.Position0.Z = 0;

            vertex.Position1.X = x + Tile.TileSize;
            vertex.Position1.Y = y + (Tile.TileSizeHalf - rect.Bottom);
            vertex.Position1.Z = 0;

            vertex.Position2.X = x;
            vertex.Position2.Y = y + (Tile.TileSizeHalf - rect.Top);
            vertex.Position2.Z = 0;

            vertex.Position3.X = x + Tile.TileSizeHalf;
            vertex.Position3.Y = y + (Tile.TileSize - rect.Right);
            vertex.Position3.Z = 0;

            vertex.Hue0 = vertex.Hue1 = vertex.Hue2 = vertex.Hue3 = hue;

            return PushSprite(texture);
        }

        public void End()
        {
            EnsureStarted();
            Flush();
            _isStarted = false;
            _customEffect = null;
        }

        private void ApplyStates()
        {
            _graphicsDevice.BlendState = _blendState;
            _graphicsDevice.DepthStencilState = _stencilState;
            _graphicsDevice.RasterizerState = false
                ? _rasterizerState
                : RasterizerState.CullNone;
            _graphicsDevice.SamplerStates[0] = _sampler;
            _graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            _graphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

            _graphicsDevice.Indices = _indexBuffer;
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);

            SetMatrixForEffect(_effect);
        }

        private void Flush()
        {
            ApplyStates();
            if (_numSprites == 0)
            {
                return;
            }

            var start = UpdateVertexBuffer(_numSprites);
            int offset = 0;

            var currentTexture = _textures[0];
            for (int i = 1; i < _numSprites; i++)
            {
                var tex = _textures[0 + i];
                if (tex != currentTexture)
                {
                    InternalDraw(currentTexture, start + offset, i - offset);
                    currentTexture = tex;
                    offset = i;
                }
            }
            InternalDraw(currentTexture, start + offset, _numSprites - offset);

            _numSprites = 0;
        }

        private void SetMatrixForEffect(MatrixEffect2 matrixEffect)
        {
            //_projectionMatrix.M11 = (float)(2.0 / _graphicsDevice.Viewport.Width);
            //_projectionMatrix.M22 = (float)(-2.0 / _graphicsDevice.Viewport.Height);

            //_projectionMatrix = Matrix.CreateOrthographic(40, 22.5f, 0.05f, 64f);

            _projectionMatrix = Matrix.Identity;

            Matrix.Multiply(ref _transformMatrix, ref _projectionMatrix, out Matrix matrix);

            matrixEffect.ApplyStates(matrix);
        }

        private void InternalDraw(Texture texture, int baseSprite, int batchSize)
        {
            _graphicsDevice.Textures[0] = texture;
            _graphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                baseSprite << 2,
                0,
                batchSize << 2,
                0,
                batchSize << 1);
        }

        private int UpdateVertexBuffer(int length)
        {
            int position;
            SetDataOptions options;

            if (_currentBufferPosition + length > MaxSprites)
            {
                position = 0;
                options = SetDataOptions.Discard;
            }
            else
            {
                position = _currentBufferPosition;
                options = SetDataOptions.NoOverwrite;
            }

            _vertexBuffer.SetDataPointerEXT
            (
                position * VertexPositionNormalTextureColor4.SizeInBytes, (IntPtr)_vertices,
                length * VertexPositionNormalTextureColor4.SizeInBytes, options
            );

            _currentBufferPosition = position + length;

            return position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool PushSprite(Texture2D texture)
        {
            if (texture?.IsDisposed != false)
            {
                return false;
            }

            EnsureSize();
            _textures[_numSprites++] = texture;

            return true;
        }

        private void EnsureSize()
        {
            EnsureStarted();
            if (_numSprites >= MaxSprites)
            {
                Flush();
            }
        }

        [Conditional("DEBUG")]
        private void EnsureStarted()
        {
            if (!_isStarted)
            {
                throw new InvalidOperationException();
            }
        }

        [Conditional("DEBUG")]
        private void EnsureNotStarted()
        {
            if (_isStarted)
            {
                throw new InvalidOperationException();
            }
        }

        private static short[] GenerateIndices()
        {
            var result = new short[MaxIndices];
            for (int i = 0, j = 0; i < MaxIndices; i += 6, j += 4)
            {
                result[i] = (short)j;
                result[i + 1] = (short)(j + 1);
                result[i + 2] = (short)(j + 2);
                result[i + 3] = (short)(j + 1);
                result[i + 4] = (short)(j + 3);
                result[i + 5] = (short)(j + 2);
            }

            return result;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct VertexPositionNormalTextureColor4 : IVertexType
        {
            public Vector3 Position0;
            public Vector3 Normal0;
            public Vector3 TextureCoordinate0;
            public Vector3 Hue0;

            public Vector3 Position1;
            public Vector3 Normal1;
            public Vector3 TextureCoordinate1;
            public Vector3 Hue1;

            public Vector3 Position2;
            public Vector3 Normal2;
            public Vector3 TextureCoordinate2;
            public Vector3 Hue2;

            public Vector3 Position3;
            public Vector3 Normal3;
            public Vector3 TextureCoordinate3;
            public Vector3 Hue3;

            VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

            private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float) * 9, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1)
            );

            public const int SizeInBytes = sizeof(float) * 12 * 4;
        }
    }
}
