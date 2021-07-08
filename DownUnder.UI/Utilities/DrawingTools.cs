using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DownUnder.UI.Utilities
{
    public static class DrawingTools
    {
        public static void DrawBorder(Texture2D white_dot, SpriteBatch sprite_batch, Rectangle rectangle, float thickness, Color color, Directions2D? sides = null)
        {
            DrawBorder(white_dot, sprite_batch, rectangle, (int)thickness, color, sides);
        }

        public static void DrawBorder(Texture2D white_dot, SpriteBatch sprite_batch, Rectangle rectangle, int thickness, Color color, Directions2D? sides = null)
        {
            Directions2D l_sides;
            if (sides == null) { l_sides = Directions2D.UDLR; }
            else { l_sides = sides.Value; }

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