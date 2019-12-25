using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DownUnder.Utility
{
    public static class DrawingTools
    {
        public static Texture2D WhiteDot(GraphicsDevice graphics)
        {
            return Dot(graphics, Color.White);
        }

        public static Texture2D Dot(GraphicsDevice graphics, Color color)
        {
            Texture2D white_dot = new Texture2D(graphics, 1, 1);
            white_dot.SetData(new[] { color });
            return white_dot;
        }

        public static void DrawBorder(Texture2D white_dot, SpriteBatch sprite_batch, Rectangle rectangle, float thickness, Color color, Directions2D sides = null)
        {
            DrawBorder(white_dot, sprite_batch, rectangle, (int)thickness, color, sides);
        }

        public static void DrawBorder(Texture2D white_dot, SpriteBatch sprite_batch, Rectangle rectangle, int thickness, Color color, Directions2D sides = null)
        {
            Directions2D l_sides;
            if (sides == null) { l_sides = Directions2D.UpDownLeftRight; }
            else { l_sides = sides; }

            if (l_sides.Up)
                sprite_batch.Draw(white_dot, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);

            if (l_sides.Left)
                sprite_batch.Draw(white_dot, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);

            if (l_sides.Right)
                sprite_batch.Draw(white_dot, new Rectangle((rectangle.X + rectangle.Width - thickness),
                                            rectangle.Y,
                                            thickness,
                                            rectangle.Height), color);
            if (l_sides.Down)
                sprite_batch.Draw(white_dot, new Rectangle(rectangle.X,
                                            rectangle.Y + rectangle.Height - thickness,
                                            rectangle.Width,
                                            thickness), color);
        }
    }
}