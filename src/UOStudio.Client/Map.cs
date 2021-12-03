using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using UOStudio.TextureAtlasGenerator.Client;

namespace UOStudio.Client
{
    public class Map : IDisposable
    {
        private readonly ILogger _logger;
        private VertexBuffer _landVertexBuffer;
        private VertexBuffer _itemVertexBuffer;
        private TextureAtlas _textureAtlas;

        private readonly IList<VertexPositionColorTexture> _landVertices;
        private readonly IList<VertexPositionTexture> _itemVertices;

        private readonly IDictionary<(int, int), LandTile> _landTiles;
        private readonly IDictionary<(int, int), ItemTile[]> _itemTiles;

        private IDictionary<Point, MapChunk> _mapChunks;

        private Effect _mapEffect;

        public Map(ILogger logger)
        {
            _logger = logger.ForContext<Map>();
            _landVertices = new List<VertexPositionColorTexture>();
            _itemVertices = new List<VertexPositionTexture>();
            _landTiles = new Dictionary<(int, int), LandTile>();
            _itemTiles = new Dictionary<(int, int), ItemTile[]>();
        }

        public void Dispose()
        {
            _mapEffect?.Dispose();
            _landVertexBuffer?.Dispose();
            _itemVertexBuffer?.Dispose();
            _textureAtlas?.Dispose();
        }

        public void Draw(GraphicsDevice graphicsDevice, Camera camera)
        {
            var world = Matrix.Identity * Matrix.CreateScale(new Vector3(1, -1, 1));
            var view = camera.ViewMatrix;
            var projection = camera.ProjectionMatrix;

            _mapEffect.Parameters["M_WorldViewProj"].SetValue(world * view * projection);
            _mapEffect.CurrentTechnique.Passes[0].Apply();

            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            if (graphicsDevice.Textures[0] != _textureAtlas!.AtlasTexture)
            {
                graphicsDevice.Textures[0] = _textureAtlas.AtlasTexture;
            }

            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            graphicsDevice.SetVertexBuffer(_landVertexBuffer);
            graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _landVertices.Count / 3);
        }

        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, string mapDirectory, string mapName)
        {
            _mapEffect = contentManager.Load<Effect>("Effects/WorldEffect.fxc");

            _landVertices.Clear();
            _itemVertices.Clear();

            _textureAtlas = new TextureAtlas(_logger, graphicsDevice, mapName);
            _textureAtlas.LoadContent(contentManager);
            LoadMapTiles(mapDirectory);

            _landVertexBuffer?.Dispose();
            _landVertexBuffer = BuildLandVertexBuffer(graphicsDevice, 64, 64);

            _itemVertexBuffer?.Dispose();
        }

        private void LoadMapTiles(string mapDirectory)
        {
            _landTiles.Clear();
            _itemTiles.Clear();

            var map = new Engine.Ultima.Map(mapDirectory, "Felucca");
            var tm = map.Tiles;
            var w = 8;
            var h = 8;
            var startx = 144;
            var starty = 192;
            var rnd = new Random();

            for (var bx = startx; bx < startx + w; bx++)
            {
                for (var by = starty; by < starty + h; by++)
                {
                    var lb = tm.GetLandBlock(bx, by);
                    var sb = tm.GetStaticBlock(bx, by);

                    var bbx = bx - startx;
                    var bby = by - starty;

                    for (var cx = 0; cx < 8; cx++)
                    {
                        for (var cy = 0; cy < 8; cy++)
                        {
                            var lt = lb[cy * 8 + cx];
                            var st = sb[cy][cx];
                            var x = bbx * 8 + cx;
                            var y = bby * 8 + cy;
                            //_landTiles.Add((x, y), new LandTile(rnd.Next(0x5f1, 0x611), lt.Z));
                            _landTiles.Add((x, y), new LandTile(lt.ID, lt.Z));

                            //var tiles = new Tile[st.Length];
                            //for (var staticIndex = 0; staticIndex < st.Length; staticIndex++)
                            //{
                            //    tiles[staticIndex] = new Tile(st[staticIndex].Id, st[staticIndex].Z);
                            //}
                            //_mapStaticTiles.Add((x, y), tiles);

                        }
                    }
                }
            }
        }

        private float GetTileZ(int x, int y, float defaultZ) => _landTiles.TryGetValue((x, y), out var tile)
            ? tile.Z
            : defaultZ;

        private VertexBuffer BuildLandVertexBuffer(GraphicsDevice graphicsDevice, int width, int height)
        {
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
                var tile = _textureAtlas.GetLandTile(landId);
                */

                var tile = tileZ != 0
                    ? _textureAtlas.GetLandTile(landId)
                    : _textureAtlas.GetLandTextureTile(landId);

                var p0 = new Vector3(x + TileSizeHalf, y - tileZ * 4, tile.Uvws.V1.W);
                var p1 = new Vector3(x + TileSize, y + TileSizeHalf - tileZEast * 4, tile.Uvws.V2.W);
                var p2 = new Vector3(x, y + TileSizeHalf - tileZWest * 4, tile.Uvws.V3.W);
                var p3 = new Vector3(x + TileSizeHalf, y + TileSize - tileZSouth * 4, tile.Uvws.V4.W);

                var uv0 = new Vector2(tile.Uvws.V1.U, tile.Uvws.V1.V);
                var uv1 = new Vector2(tile.Uvws.V2.U, tile.Uvws.V2.V);
                var uv2 = new Vector2(tile.Uvws.V3.U, tile.Uvws.V3.V);
                var uv3 = new Vector2(tile.Uvws.V4.U, tile.Uvws.V4.V);

                var n1 = Vector3.Cross(p2 - p0, p1 - p0);
                _landVertices.Add(new VertexPositionColorTexture(p0, Color.Red, uv0));
                _landVertices.Add(new VertexPositionColorTexture(p1, Color.Red, uv1));
                _landVertices.Add(new VertexPositionColorTexture(p2, Color.Red, uv2));

                var n2 = Vector3.Cross(p2 - p3, p1 - p3);
                _landVertices.Add(new VertexPositionColorTexture(p1, Color.Black, uv1));
                _landVertices.Add(new VertexPositionColorTexture(p3, Color.Green, uv3));
                _landVertices.Add(new VertexPositionColorTexture(p2, Color.Black, uv2));
            }

            for (var x = 0; x < width; ++x)
            {
                for (var y = 0; y < height; ++y)
                {
                    var tileZ = GetTileZ(x, y, -15);
                    var tileZEast = GetTileZ(x + 1, y, tileZ);
                    var tileZWest = GetTileZ(x, y + 1, tileZ);
                    var tileZSouth = GetTileZ(x + 1, y + 1, tileZ);
                    var graphicId = _landTiles[(x, y)].TileId;

                    DrawSquare(graphicId, (x - y) * TileSizeHalf, (x + y) * TileSizeHalf, tileZ, tileZEast, tileZWest, tileZSouth);
                }
            }

            var vertexBuffer = new VertexBuffer(
                graphicsDevice,
                typeof(VertexPositionColorTexture),
                _landVertices.Count,
                BufferUsage.WriteOnly
            );
            vertexBuffer.SetData(_landVertices.ToArray());
            return vertexBuffer;
        }

        private readonly struct LandTile
        {
            public LandTile(int tileId, int z)
            {
                TileId = tileId;
                Z = z;
            }

            public int TileId { get; }

            public int Z { get; }
        }

        private readonly struct ItemTile
        {
            public ItemTile(int tileId, int z, int height)
            {
                TileId = tileId;
                Z = z;
                Height = height;
            }

            public int TileId { get; }

            public int Height { get; }

            public int Z { get; }
        }
    }
}
