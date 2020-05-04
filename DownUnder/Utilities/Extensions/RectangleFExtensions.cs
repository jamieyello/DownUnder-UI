using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utility;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace DownUnder {
    public static class RectangleFExtensions {
        public static Rectangle ToRectangle(this RectangleF r, bool round) =>
            !round ? r.ToRectangle() : new Rectangle(r.X.Rounded(), r.Y.Rounded(), r.Width.Rounded(), r.Height.Rounded());
        
        public static RectangleF SnapInsideRectangle(this RectangleF inner, RectangleF outer, DiagonalDirections2D snapping_policy) {
            inner.Position = outer.Position.WithOffset(inner.Position);

            Directions2D snapping = snapping_policy.ToPerpendicular();

            if (snapping.Left && !snapping.Right) inner.X = outer.X; // left
            if (!snapping.Left && snapping.Right) inner.X = outer.X + outer.Width - inner.Width; // right
            if (snapping.Left && snapping.Right) { // left and right
                inner.X = outer.X;
                inner.Width = outer.Width;
            }
            if (snapping.Up && !snapping.Down) inner.Y = outer.Y; // up
            if (!snapping.Up && snapping.Down) inner.Y = outer.Y + outer.Height - inner.Height; // down
            if (snapping.Up && snapping.Down) { // up and down
                inner.Y = outer.Y;
                inner.Height = outer.Height;
            }

            return inner;
        }

        public static RectangleF WithOffset(this RectangleF r, Point2 p) {
            RectangleF result = r;
            result.Offset(p);
            return result;
        }

        public static RectangleF WithCenter(this RectangleF r, Point2 center) =>
            r.WithPosition(center.WithOffset(r.Size.ToPoint2().MultipliedBy(-0.5f)));

        public static RectangleF WithCenter(this RectangleF r, RectangleF r2) => 
            r.WithCenter(r2.Center);
        
        /// <summary> Returns a new <see cref="RectangleF"/> positioned inside a given <see cref="RectangleF"/> with aspect ratio preserved. </summary>
        public static RectangleF FittedIn(this RectangleF r, RectangleF r2, float min_spacing = 0f) {
            float max_size = Math.Min(
                    Math.Min(r.Width, r2.Width - min_spacing),
                    Math.Min(r.Height, r2.Height - min_spacing));

            return new RectangleF(0f, 0f,
                    r.Width * (max_size / r.Width),
                    r.Height * (max_size / r.Height))
                    .WithCenter(r2);
        }

        /// <summary> Calculates the distance between a point and a rectangle. </summary>
        public static double DistanceFrom(this RectangleF rectangle, Point2 point) {
            if (rectangle.Contains(point)) return 0;
            var dx = Math.Max(Math.Max(rectangle.X - point.X, point.X - rectangle.Right), 0);
            var dy = Math.Max(Math.Max(rectangle.Top - point.Y, point.Y - rectangle.Bottom), 0);
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary> Returns a new <see cref="RectangleF"/> that's been resized by X pixels. (On all sides) </summary>
        public static RectangleF ResizedBy(this RectangleF r, float x, float y) {
            RectangleF result = r;
            result.X -= x;
            result.Y -= y;
            result.Width += x * 2;
            result.Height += y * 2;
            return result;
        }

        public static RectangleF ResizedBy(this RectangleF r, BorderSize bs) =>
            new RectangleF(
                r.X - bs.Left,
                r.Y - bs.Top,
                r.Width + bs.Left + bs.Right,
                r.Height + bs.Top + bs.Down
                );
        
        public static RectangleF ResizedBy(this RectangleF r, float amount, Directions2D directions) {
            RectangleF result = r;
            if (directions & Directions2D.R) result.Width += amount;
            if (directions & Directions2D.D) result.Height += amount;
            if (directions & Directions2D.U) {
                result.Y -= amount;
                result.Height += amount;
            }
            if (directions & Directions2D.L) {
                result.X -= amount;
                result.Width += amount;
            }

            return result;
        }

        /// <summary> Returns a new <see cref="RectangleF"/> that's had its size multiplied by the given modifier. </summary>
        public static RectangleF ResizedBy(this RectangleF r, Point2 modifier) => new RectangleF(r.Position, r.Size.ToPoint2().MultipliedBy(modifier));
        
        /// <summary> Returns a new <see cref="RectangleF"/> that's had its size multiplied by the given modifier. </summary>
        public static RectangleF ResizedBy(this RectangleF r, float modifier) => new RectangleF(r.Position, r.Size.ToPoint2().MultipliedBy(modifier));
        
        /// <summary> Returns a new RectangleF without the position values. </summary>
        public static RectangleF SizeOnly(this RectangleF r) => new RectangleF(new Point2(), r.Size);
        public static RectangleF WithX(this RectangleF r, float x) => new RectangleF(x, r.Y, r.Width, r.Height);
        public static RectangleF WithY(this RectangleF r, float y) => new RectangleF(r.X, y, r.Width, r.Height);
        public static RectangleF WithWidth(this RectangleF r, float width) => new RectangleF(r.X, r.Y, width, r.Height);
        public static RectangleF WithHeight(this RectangleF r, float height) => new RectangleF(r.X, r.Y, r.Width, height);
        public static RectangleF WithPosition(this RectangleF r, Point2 p) => new RectangleF(p, r.Size);
        public static RectangleF WithSize(this RectangleF r, Size2 s) => new RectangleF(r.Position, s);
        public static RectangleF WithSize(this RectangleF r, float width, float height) => new RectangleF(r.X, r.Y, width, height);
        
        public static RectangleF WithMinimumSize(this RectangleF r, Point2 s) {
            RectangleF result = r;
            if (r.Width < s.X) result.Width = s.X;
            if (r.Height < s.Y) result.Height = s.Y;
            return result;
        }

        public static RectangleF WithMaximumSize(this RectangleF r, Point2 s) {
            RectangleF result = r;
            if (r.Width > s.X) result.Width = s.X;
            if (r.Height > s.Y) result.Height = s.Y;
            return result;
        }

        // temp extensions until MonoGame.Extended adds these missing properties
        public static Point2 TopRight(this RectangleF r) => new Point2(r.X + r.Width, r.Y);
        
        public static Point2 BottomLeft(this RectangleF r) => new Point2(r.X, r.Y + r.Height);
        
        public static Directions2D GetCursorHoverOnBorders(this RectangleF r, Point2 p, float border_thickness) {
            RectangleF[] areas = GetBorderArea(r, border_thickness);
            Directions2D result = new Directions2D();

            if (areas[0].Contains(p)) result.Up = true;
            if (areas[1].Contains(p)) result.Right = true;
            if (areas[2].Contains(p)) result.Down = true;
            if (areas[3].Contains(p)) result.Left = true;

            return result;
        }

        public static RectangleF[] GetBorderArea(this RectangleF r, float thickness) {
            RectangleF[] areas = new RectangleF[4];
            float half_thickness = thickness / 2;

            areas[0] = new RectangleF(r.X - half_thickness, r.Y - half_thickness, r.Width + thickness, thickness); // Top
            areas[1] = new RectangleF(r.X + r.Width - half_thickness, r.Y - half_thickness, thickness, r.Height + thickness); // Right
            areas[2] = new RectangleF(r.X - half_thickness, r.Y + r.Height - half_thickness, r.Width + thickness, thickness); // Bottom
            areas[3] = new RectangleF(r.X - half_thickness, r.Y - half_thickness, thickness, r.Height + thickness); //Left

            return areas;
        }

        /// <summary> Performs linear interpolation of <see cref="RectangleF"/>. </summary>
        /// <param name="progress">Value between 0f and 1f to represent the progress between the two <see cref="RectangleF"/>s</param>
        public static RectangleF Lerp(this RectangleF r1, RectangleF r2, float progress) {
            float inverse_progress = 1 - progress;
            return new RectangleF (
                r1.X * inverse_progress + r2.X * progress,
                r1.Y * inverse_progress + r2.Y * progress,
                r1.Width * inverse_progress + r2.Width * progress,
                r1.Height * inverse_progress + r2.Height * progress
                );
        }

        public static RectangleF Rounded(this RectangleF r, float accuracy) =>
            new RectangleF(
                ((int)((r.X + accuracy / 2) / accuracy) * accuracy),
                ((int)((r.Y + accuracy / 2) / accuracy) * accuracy),
                ((int)((r.Width + accuracy / 2) / accuracy) * accuracy),
                ((int)((r.Height + accuracy / 2) / accuracy) * accuracy)
                );
        
    }
}
