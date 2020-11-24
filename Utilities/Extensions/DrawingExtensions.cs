using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace DownUnder
{
    public static class DrawingExtensions
    {
        public static void DrawRoundedRect(this SpriteBatch spriteBatch, RectangleF rect, float radius, Color color, float thickness = 1f)
        {
            // Top left round
            DrawCircleQuarters(spriteBatch, rect.Position.WithOffset(new Point2(radius, radius)), radius, DiagonalDirections2D.TL, color, 1f, 1f, thickness);

            // Top line
            spriteBatch.DrawLine(rect.X + radius, rect.Y, rect.X + rect.Width - radius, rect.Y, color, thickness);

            // Top right round
            DrawCircleQuarters(spriteBatch, rect.TopRight().WithOffset(new Point2(-radius, radius)), radius, DiagonalDirections2D.TR, color, 1f, 1f, thickness);

            // Right line
            spriteBatch.DrawLine(rect.X + rect.Width, rect.Y + radius, rect.X + rect.Width, rect.Y + rect.Height - radius, color, thickness);

            // Bottom right round
            DrawCircleQuarters(spriteBatch, rect.BottomRight.WithOffset(new Point2(-radius, -radius)), radius, DiagonalDirections2D.BR, color, 1f, 1f, thickness);

            // Bottom line
            spriteBatch.DrawLine(rect.X + radius, rect.Y + rect.Height, rect.X + rect.Width - radius, rect.Y + rect.Height, color, thickness);

            // Bottom left round
            DrawCircleQuarters(spriteBatch, rect.BottomLeft().WithOffset(new Point2(radius, -radius)), radius, DiagonalDirections2D.BL, color, 1f, 1f, thickness);

            // Left line
            spriteBatch.DrawLine(rect.X, rect.Y + radius, rect.X, rect.Y + rect.Height - radius, color, thickness);
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
            if (directions.BottomRight)
            {
                _DrawCircleQuarter(spriteBatch, center, radius, color, stretch_x, stretch_y, thickness);
            }
            if (directions.BottomLeft)
            {
                _DrawCircleQuarter(spriteBatch, center, radius, color, -stretch_x, stretch_y, thickness);
            }
            if (directions.TopRight)
            {
                _DrawCircleQuarter(spriteBatch, center, radius, color, stretch_x, -stretch_y, thickness);
            }
            if (directions.TopLeft)
            {
                _DrawCircleQuarter(spriteBatch, center, radius, color, -stretch_x, -stretch_y, thickness);
            }
        }

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
            DrawLines(spriteBatch, GetCircleQuarterPoints(center, radius, mod_x, mod_y), color, thickness);
        }

        // https://www.mathopenref.com/coordbasiccircle.html
        public static List<Point2> GetCircleQuarterPoints
            (
                Point2 center,
                float radius,
                float mod_x = 1f,
                float mod_y = 1f
            )
        {
            float accuracy = radius / 2;
            float r_s = radius * radius;
            List<Point2> points = new List<Point2>();

            float x;
            for (int i = 0; i < accuracy; i++)
            {
                x = (i / accuracy) * radius;
                // solve for y (y = ± √(r2−x2))
                points.Add(new Point2(x * mod_x, (float)Math.Sqrt(r_s - x * x) * mod_y).WithOffset(center));
            }
            points.Add(new Point2(radius * mod_x + center.X, center.Y));

            return points;
        }

        public static void DrawLines(this SpriteBatch spriteBatch, List<Point2> points, Color color, float thickness = 1f)
        {
            if (points.Count == 0) return;
            if (points.Count == 1)
            {
                spriteBatch.DrawPoint(points[0], color, thickness);
            }
            for (int i = 1; i < points.Count; i++)
            {
                spriteBatch.DrawLine(points[i - 1].X, points[i - 1].Y, points[i].X, points[i].Y, color, thickness);
            }
        }

        public static void DrawQuad(this SpriteBatch spriteBatch, Vector2 position, IReadOnlyList<Vector2> points, Color color, float layer_depth = 0f)
        {
            
            spriteBatch.DrawPolygon(position, new MonoGame.Extended.Shapes.Polygon(new Vector2[] { points[0], points[1], points[2] }), color, layer_depth);
            spriteBatch.DrawPolygon(position, new MonoGame.Extended.Shapes.Polygon(new Vector2[] { points[3], points[1], points[2] }), color, layer_depth);
        }
    }
}
