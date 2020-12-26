using System;
using Microsoft.Xna.Framework.Graphics;

namespace UOStudio.Client.Engine
{
    public sealed class StaticTile : Tile
    {
        public StaticTile(ushort graphicId, ushort x, ushort y, sbyte staticZ)
            : base(graphicId, x, y, staticZ: staticZ)
        {
        }

        public override void Draw(TileBatcher batcher, Texture2D texture)
        {
            throw new NotImplementedException();
        }
    }
}
