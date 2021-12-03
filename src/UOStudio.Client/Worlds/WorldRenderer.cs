using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UOStudio.Common.Network;

namespace UOStudio.Client.Worlds
{
    internal sealed class WorldRenderer : IWorldRenderer
    {
        private GraphicsDevice _graphicsDevice;

        private VertexBuffer _staticTileVertexBuffer;
        private VertexBuffer _itemTileVertexBuffer;
        private Effect _mapEffect;

        private readonly IDictionary<Point, VertexBuffer> _vertexBufferCache;

        public WorldRenderer()
        {
            _vertexBufferCache = new Dictionary<Point, VertexBuffer>(256);
        }

        public void Dispose()
        {
            _staticTileVertexBuffer?.Dispose();
            _itemTileVertexBuffer?.Dispose();
        }

        public void Draw(GraphicsDevice graphicsDevice, World world, Camera camera)
        {
            var modelMatrix = Matrix.Identity * Matrix.CreateScale(new Vector3(1, -1, 1));
            var view = camera.ViewMatrix;
            var projection = camera.ProjectionMatrix;

            _mapEffect.Parameters["M_WorldViewProj"].SetValue(modelMatrix * view * projection);
            _mapEffect.CurrentTechnique.Passes[0].Apply();

            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            /*
            if (graphicsDevice.Textures[0] != _textureAtlas!.AtlasTexture)
            {
                graphicsDevice.Textures[0] = _textureAtlas.AtlasTexture;
            }
            */

            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (var kvp in _vertexBufferCache)
            {
                graphicsDevice.SetVertexBuffer(kvp.Value);
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, kvp.Value.VertexCount / 3);
            }
        }

        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _mapEffect = contentManager.Load<Effect>("Effects/WorldEffect.fxc");
            _graphicsDevice = graphicsDevice;
        }

        public void Update(World world, Camera camera)
        {
            var visibleChunks = GetVisibleChunks(camera, world);
        }

        public void UpdateChunk(WorldChunk worldChunk)
        {
            if (!worldChunk.IsDirty)
            {
                return;
            }

            if (_vertexBufferCache.TryGetValue(worldChunk.Position, out var worldChunkVertexBuffer))
            {
                worldChunkVertexBuffer.Dispose();
                _vertexBufferCache.Remove(worldChunk.Position);
            }

            worldChunkVertexBuffer = BuildVertexBuffer(worldChunk);
            _vertexBufferCache.Add(worldChunk.Position, worldChunkVertexBuffer);
        }

        private VertexBuffer BuildVertexBuffer(WorldChunk worldChunk)
        {
            var landVertices = new List<VertexPositionColorTexture>();

            const int TileSize = 44;
            const int TileSizeHalf = TileSize / 2;

            void DrawSquare(int landId, int x, int y, float tileZ, float tileZEast, float tileZWest, float tileZSouth)
            {
                /*
                var tile = tileZ == 0
                    ? _textureAtlas.GetLandTile(landId)
                    : _textureAtlas.GetLandTextureTile(landId);
                    */

                /*
                var tile = tileZ != 0
                    ? _textureAtlas.GetLandTile(landId)
                    : _textureAtlas.GetLandTextureTile(landId);
                    */

                /* uncomment when atlas is in
                var p0 = new Vector3(x + TileSizeHalf, y - tileZ * 4, tile.Uvws.V1.W);
                var p1 = new Vector3(x + TileSize, y + TileSizeHalf - tileZEast * 4, tile.Uvws.V2.W);
                var p2 = new Vector3(x, y + TileSizeHalf - tileZWest * 4, tile.Uvws.V3.W);
                var p3 = new Vector3(x + TileSizeHalf, y + TileSize - tileZSouth * 4, tile.Uvws.V4.W);
                */

                var p0 = new Vector3(x + TileSizeHalf, y - tileZ * 4, 0);
                var p1 = new Vector3(x + TileSize, y + TileSizeHalf - tileZEast * 4, 0);
                var p2 = new Vector3(x, y + TileSizeHalf - tileZWest * 4, 0);
                var p3 = new Vector3(x + TileSizeHalf, y + TileSize - tileZSouth * 4, 0);

                /* uncomment when atlas is in
                var uv0 = new Vector2(tile.Uvws.V1.U, tile.Uvws.V1.V);
                var uv1 = new Vector2(tile.Uvws.V2.U, tile.Uvws.V2.V);
                var uv2 = new Vector2(tile.Uvws.V3.U, tile.Uvws.V3.V);
                var uv3 = new Vector2(tile.Uvws.V4.U, tile.Uvws.V4.V);
                */
                var uv0 = Vector2.Zero;
                var uv1 = Vector2.Zero;
                var uv2 = Vector2.Zero;
                var uv3 = Vector2.Zero;

                var n1 = Vector3.Cross(p2 - p0, p1 - p0);
                landVertices.Add(new VertexPositionColorTexture(p0, Color.Red, uv0));
                landVertices.Add(new VertexPositionColorTexture(p1, Color.Red, uv1));
                landVertices.Add(new VertexPositionColorTexture(p2, Color.Red, uv2));

                var n2 = Vector3.Cross(p2 - p3, p1 - p3);
                landVertices.Add(new VertexPositionColorTexture(p1, Color.Black, uv1));
                landVertices.Add(new VertexPositionColorTexture(p3, Color.Green, uv3));
                landVertices.Add(new VertexPositionColorTexture(p2, Color.Black, uv2));
            }

            for (var x = 0; x < ChunkData.ChunkSize; ++x)
            {
                for (var y = 0; y < ChunkData.ChunkSize; ++y)
                {
                    var staticTile = worldChunk.GetStaticTile(x, y);

                    var tileZ = worldChunk.GetZ(x, y);
                    var tileZEast = worldChunk.GetZ(x + 1, y);
                    var tileZWest = worldChunk.GetZ(x, y + 1);
                    var tileZSouth = worldChunk.GetZ(x + 1, y + 1);

                    DrawSquare(staticTile.TileId, (x - y) * TileSizeHalf, (x + y) * TileSizeHalf, tileZ, tileZEast, tileZWest, tileZSouth);
                }
            }

            var vertexBuffer = new VertexBuffer(
                _graphicsDevice,
                typeof(VertexPositionColorTexture),
                landVertices.Count,
                BufferUsage.WriteOnly
            );
            vertexBuffer.SetData(landVertices.ToArray());
            return vertexBuffer;
        }

        private IReadOnlyCollection<WorldChunk> GetVisibleChunks(Camera camera, World world)
        {
            return null;
        }
    }
}
