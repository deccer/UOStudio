using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace UOStudio.Client.Core.Extensions
{
    public static class ShapeExtensions
    {
        private static Texture2D _whitePixelTexture;

        private static Texture2D GetTexture(GraphicsResource spriteBatch)
        {
            if (_whitePixelTexture == null)
            {
                _whitePixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                _whitePixelTexture.SetData(new[] { Color.White });
            }

            return _whitePixelTexture;
        }

        public static void DrawPolygon(this SpriteBatch spriteBatch, Vector2 offset, IReadOnlyList<Vector2> points, Color color, float thickness = 1f)
        {
            if (points.Count == 0)
            {
                return;
            }

            if (points.Count == 1)
            {
                DrawPoint(spriteBatch, points[0], color, (int)thickness);
                return;
            }

            var texture = GetTexture(spriteBatch);

            for (var i = 0; i < points.Count - 1; i++)
            {
                DrawPolygonEdge(spriteBatch, texture, points[i] + offset, points[i + 1] + offset, color, thickness);
            }

            DrawPolygonEdge(spriteBatch, texture, points[points.Count - 1] + offset, points[0] + offset, color, thickness);
        }

        private static void DrawPolygonEdge(SpriteBatch spriteBatch, Texture2D texture, Vector2 point1, Vector2 point2, Color color, float thickness)
        {
            var length = Vector2.Distance(point1, point2);
            var angle = (float)System.Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(texture, point1, null, color, angle, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        public static void FillRectangle(this SpriteBatch spriteBatch, RectangleF rectangle, Color color)
        {
            FillRectangle(spriteBatch, new Vector2(rectangle.Location.X, rectangle.Location.Y), rectangle.Size, color);
        }

        public static void FillRectangle(this SpriteBatch spriteBatch, Vector2 location, SizeF size, Color color)
        {
            spriteBatch.Draw(GetTexture(spriteBatch), location, null, color, 0, Vector2.Zero, new Vector2(size.Width, size.Height), SpriteEffects.None, 0);
        }

        public static void FillRectangle(this SpriteBatch spriteBatch, float x, float y, float width, float height, Color color)
        {
            FillRectangle(spriteBatch, new Vector2(x, y), new SizeF(width, height), color);
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, RectangleF rectangle, Color color, float thickness = 1f)
        {
            var texture = GetTexture(spriteBatch);
            var topLeft = new Vector2(rectangle.X, rectangle.Y);
            var topRight = new Vector2(rectangle.Right - thickness, rectangle.Y);
            var bottomLeft = new Vector2(rectangle.X, rectangle.Bottom - thickness);
            var horizontalScale = new Vector2(rectangle.Width, thickness);
            var verticalScale = new Vector2(thickness, rectangle.Height);

            spriteBatch.Draw(texture, topLeft, null, color, 0f, Vector2.Zero, horizontalScale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, topLeft, null, color, 0f, Vector2.Zero, verticalScale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, topRight, null, color, 0f, Vector2.Zero, verticalScale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, bottomLeft, null, color, 0f, Vector2.Zero, horizontalScale, SpriteEffects.None, 0);
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 location, SizeF size, Color color, float thickness = 1f)
        {
            DrawRectangle(spriteBatch, new RectangleF(location.X, location.Y, size.Width, size.Height), color, thickness);
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, float x, float y, float width, float height, Color color, float thickness = 1f)
        {
            DrawRectangle(spriteBatch, new RectangleF(x, y, width, height), color, thickness);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, float thickness = 1f)
        {
            DrawLine(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), color, thickness);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            // calculate the distance between the two vectors
            var distance = Vector2.Distance(point1, point2);

            // calculate the angle between the two vectors
            var angle = (float)System.Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public static void DrawPoint(this SpriteBatch spriteBatch, float x, float y, Color color, float size = 1f)
        {
            DrawPoint(spriteBatch, new Vector2(x, y), color, size);
        }

        public static void DrawPoint(this SpriteBatch spriteBatch, Vector2 position, Color color, float size = 1f)
        {
            var scale = Vector2.One * size;
            var offset = new Vector2(0.5f) - new Vector2(size * 0.5f);
            spriteBatch.Draw(GetTexture(spriteBatch), position + offset, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color, float thickness = 1f)
        {
            DrawPolygon(spriteBatch, center, CreateCircle(radius, sides), color, thickness);
        }

        public static void DrawCircle(this SpriteBatch spriteBatch, float x, float y, float radius, int sides, Color color, float thickness = 1f)
        {
            DrawPolygon(spriteBatch, new Vector2(x, y), CreateCircle(radius, sides), color, thickness);
        }

        private static Vector2[] CreateCircle(double radius, int sides)
        {
            const double max = 2.0 * System.Math.PI;
            var points = new Vector2[sides];
            var step = max / sides;
            var theta = 0.0;

            for (var i = 0; i < sides; i++)
            {
                points[i] = new Vector2((float)(radius * System.Math.Cos(theta)), (float)(radius * System.Math.Sin(theta)));
                theta += step;
            }

            return points;
        }
    }
}
