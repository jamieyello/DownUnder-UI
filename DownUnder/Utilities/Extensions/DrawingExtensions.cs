using DownUnder.Utility;
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
        public static void DrawRoundedRect(this SpriteBatch spriteBatch, RectangleF rect, float radius, Color color, float thickness = 1f)
        {
            if (radius == 0f)
            {
                spriteBatch.DrawRectangle(rect, color, thickness / 2);
            }

            // Top left round
            DrawCircleQuarters(spriteBatch, rect.Position.WithOffset(new Point2(radius, radius)), radius, DiagonalDirections2D.TopLeft, color, 1f, 1f, thickness);

            // Top line
            spriteBatch.DrawLine(rect.X + radius, rect.Y, rect.X + rect.Width - radius, rect.Y, color, thickness);

            // Top right round
            DrawCircleQuarters(spriteBatch, rect.TopRight().WithOffset(new Point2(-radius, radius)), radius, DiagonalDirections2D.TopRight, color, 1f, 1f, thickness);

            // Right line
            spriteBatch.DrawLine(rect.X + rect.Width, rect.Y + radius, rect.X + rect.Width, rect.Y + rect.Height - radius, color, thickness);

            // Bottom right round
            DrawCircleQuarters(spriteBatch, rect.BottomRight.WithOffset(new Point2(-radius, -radius)), radius, DiagonalDirections2D.BottomRight, color, 1f, 1f, thickness);

            // Bottom line
            spriteBatch.DrawLine(rect.X + radius, rect.Y + rect.Height, rect.X + rect.Width - radius, rect.Y + rect.Height, color, thickness);

            // Bottom left round
            DrawCircleQuarters(spriteBatch, rect.BottomLeft().WithOffset(new Point2(radius, -radius)), radius, DiagonalDirections2D.BottomLeft, color, 1f, 1f, thickness);

            // Left line
            spriteBatch.DrawLine(rect.X, rect.Y + radius, rect.X, rect.Y + rect.Height - radius, color, thickness);
        }

        // temp extensions until MonoGame.Extended adds these missing properties
        public static Point2 TopRight(this RectangleF r)
        {
            return new Point2(r.X + r.Width, r.Y);
        }
        public static Point2 BottomLeft(this RectangleF r)
        {
            return new Point2(r.X, r.Y + r.Height);
        }

        public static void DrawCircleQuarters
            (
                this SpriteBatch spriteBatch, 
                Point2 center, 
                float radius,
                DiagonalDirections2D directions,
                Color color,
                float stretch_x = 1f,
                float stretch_y = 1f,
                float thickness = 1f
            )
        {
            if (directions.bottom_right)
            {
                _DrawCircleQuarter(spriteBatch, center, radius, color, stretch_x, stretch_y, thickness);
            }
            if (directions.bottom_left)
            {
                _DrawCircleQuarter(spriteBatch, center, radius, color, -stretch_x, stretch_y, thickness);
            }
            if (directions.top_right)
            {
                _DrawCircleQuarter(spriteBatch, center, radius, color, stretch_x, -stretch_y, thickness);
            }
            if (directions.top_left)
            {
                _DrawCircleQuarter(spriteBatch, center, radius, color, -stretch_x, -stretch_y, thickness);
            }
        }

        // https://www.mathopenref.com/coordbasiccircle.html
        private static void _DrawCircleQuarter
            (
                this SpriteBatch spriteBatch,
                Point2 center,
                float radius,
                Color color,
                float mod_x = 1f,
                float mod_y = 1f,
                float thickness = 1f
            )
        {
            float accuracy = radius / 2;
            float r_s = radius * radius;
            Point2[] points = new Point2[(int)accuracy + 1];

            float x;
            for (int i = 0; i <= accuracy; i++)
            {
                x = (i / accuracy) * radius;
                // solve for y (y = ± √(r2−x2))
                points[i] = new Point2(x * mod_x, (float)Math.Sqrt(r_s - x * x) * mod_y).WithOffset(center);
            }

            DrawLines(spriteBatch, points, color, thickness);
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
