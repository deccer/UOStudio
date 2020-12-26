using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UOStudio.Client.Engine
{
    public abstract class Tile
    {
        public const int TileSize = 44;
        public const int TileSizeHalf = TileSize / 2;

        private Vector2 _screenPosition;

        protected Tile(ushort graphicId, ushort x, ushort y, sbyte z = 0, sbyte staticZ = 0)
        {
            GraphicId = graphicId;
            X = x;
            Y = y;
            Z = z;
            StaticZ = staticZ;
        }

        public ushort X { get; set; }

        public ushort Y { get; set; }

        public sbyte Z { get; set; }

        protected sbyte StaticZ { get; set; }

        public ushort GraphicId { get; }

        public Vector2 RealScreenPosition { get; private set; }

        public void UpdateScreenPosition()
        {
            _screenPosition = new Vector2((X - Y) * TileSizeHalf, (X + Y) * TileSizeHalf - (Z << 2));
        }

        public void UpdateRealScreenPosition(int offsetX, int offsetY)
        {
            var offset = new Vector2(offsetX, offsetY);
            var tileSize = new Vector2(TileSizeHalf, TileSizeHalf);
            RealScreenPosition = _screenPosition - offset - tileSize;
        }

        public abstract void Draw(TileBatcher batcher, Texture2D texture);
    }
}
