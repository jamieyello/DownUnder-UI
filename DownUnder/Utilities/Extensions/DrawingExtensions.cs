using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder
{
    public static class DrawingExtensions
    {
        // https://www.mathsisfun.com/algebra/circle-equations.html
        public static void DrawRoundedRect(this SpriteBatch spriteBatch, RectangleF rectangle, float percent_rounded, Color color, float thickness = 1f)
        {
            //spriteBatch.Dr
        }

        public static void DrawLines(this SpriteBatch spriteBatch, Point2[] points, Color color, float thickness = 1f)
        {
            if (points.Length == 0) return;
            if (points.Length == 1)
            {
                spriteBatch.DrawPoint(points[0], color, thickness);
            }
            for (int i = 1; i < points.Length; i++)
            {
                spriteBatch.DrawLine(points[i - 1].X, points[i - 1].Y, points[i].X, points[i].Y, color, thickness);
            }
        }
    }
}
