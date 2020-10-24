using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace UOStudio.Client.Engine.UI
{
    public static class DrawVertexDeclaration
    {
        public static readonly int Size;
        public static readonly VertexDeclaration Declaration;

        static DrawVertexDeclaration()
        {
            unsafe { Size = sizeof(ImDrawVert); }

            var position = new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0);
            var uv = new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
            var color = new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0);
            Declaration = new VertexDeclaration(Size, position, uv, color);
        }
    }
}
