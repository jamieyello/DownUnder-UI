using System;
using DownUnder.UI.UI.Widgets.DataTypes;
using DownUnder.UI.Utilities.CommonNamespace;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DownUnder.UI.Utilities.Extensions
{
    public static class RectangleFExtensions {
        public static Rectangle ToRectangle(this RectangleF r, bool round) =>
            !round ? r.ToRectangle() : new Rectangle(r.X.Rounded(), r.Y.Rounded(), r.Width.Rounded(), r.Height.Rounded());

        public static RectangleF BorderingInside(this RectangleF inner, RectangleF outer, DiagonalDirections2D sides) {
            inner.Position = outer.Position.WithOffset(inner.Position);

            Directions2D snapping = sides.ToPerpendicular();

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

        public static RectangleF ResizedBy(this RectangleF r, BorderSize bs, Point2? minimum_size = null) {
            if (minimum_size != null) {
                if (bs.Top < minimum_size.Value.Y - r.Height) bs.Top = minimum_size.Value.Y - r.Height;
                if (bs.Left < minimum_size.Value.X - r.Width) bs.Left = minimum_size.Value.X - r.Width;

                return new RectangleF(
                    r.X - bs.Left,
                    r.Y - bs.Top,
                    r.Width + bs.Left + bs.Right,
                    r.Height + bs.Top + bs.Bottom
                    ).WithMinimumSize(minimum_size.Value);
            }

            return new RectangleF(
                r.X - bs.Left,
                r.Y - bs.Top,
                r.Width + bs.Left + bs.Right,
                r.Height + bs.Top + bs.Bottom
                );
        }


        public static RectangleF ResizedBy(this RectangleF r, float amount, Directions2D? directions = null, Point2? minimum_size = null) =>
            r.ResizedBy(new BorderSize(amount, directions ?? Directions2D.All), minimum_size);

        public static RectangleF ResizedBy(this RectangleF r, RectanglePart part)
        {
            BorderSize resize = new BorderSize();
            resize.Top = r.Height * -(1f - part.Indents.Up);
            resize.Bottom = r.Height * -(1f - part.Indents.Down);
            resize.Left = r.Width * -(1f -part.Indents.Left);
            resize.Right = r.Width * -(1f - part.Indents.Right);
            return r.ResizedBy(resize);

        }

        /// <summary> Returns a new RectangleF without the position values. </summary>
        public static RectangleF SizeOnly(this RectangleF r) => new RectangleF(new Point2(), r.Size);
        public static RectangleF WithX(this RectangleF r, float x) => new RectangleF(x, r.Y, r.Width, r.Height);
        public static RectangleF WithY(this RectangleF r, float y) => new RectangleF(r.X, y, r.Width, r.Height);
        public static RectangleF WithWidth(this RectangleF r, float width) => new RectangleF(r.X, r.Y, width, r.Height);
        public static RectangleF WithHeight(this RectangleF r, float height) => new RectangleF(r.X, r.Y, r.Width, height);
        public static RectangleF WithPosition(this RectangleF r, Point2 p) => new RectangleF(p, r.Size);
        public static RectangleF WithSize(this RectangleF r, Size2 s) => new RectangleF(r.Position, s);
        public static RectangleF WithSize(this RectangleF r, float width, float height) => new RectangleF(r.X, r.Y, width, height);
        public static RectangleF WithTop(this RectangleF r, float top) => new RectangleF(r.X, top, r.Width, r.Height);
        public static RectangleF WithBottom(this RectangleF r, float bottom) => new RectangleF(r.X, bottom - r.Height, r.Width, r.Height);
        public static RectangleF WithLeft(this RectangleF r, float left) => new RectangleF(left, r.Y, r.Width, r.Height);
        public static RectangleF WithRight(this RectangleF r, float right) => new RectangleF(right - r.Width, r.Y, r.Width, r.Height);
        public static RectangleF WithAdditionalSize(this RectangleF r, float width, float height) => new RectangleF(r.X, r.Y, r.Width + width, r.Height + height);
        public static RectangleF WithOffset(this RectangleF r, float x, float y) => new RectangleF(r.X + x, r.Y + y, r.Width, r.Height);
        public static RectangleF WithScaledSize(this RectangleF r, float x, float y) => new RectangleF(r.X, r.Y, r.Width * x, r.Height * y);
        /// <summary> Returns a new <see cref="RectangleF"/> that's had its size multiplied by the given modifier. </summary>
        public static RectangleF WithScaledSize(this RectangleF r, Point2 modifier) => new RectangleF(r.Position, r.Size.ToPoint2().MultipliedBy(modifier));
        /// <summary> Returns a new <see cref="RectangleF"/> that's had its size multiplied by the given modifier. </summary>
        public static RectangleF WithScaledSize(this RectangleF r, float modifier) => new RectangleF(r.Position, r.Size.ToPoint2().MultipliedBy(modifier));


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

        public static float GetSide(this RectangleF r, Direction2D direction)
        {
            if (direction == Direction2D.up) return r.Top;
            if (direction == Direction2D.down) return r.Bottom;
            if (direction == Direction2D.left) return r.Left;
            if (direction == Direction2D.right) return r.Right;
            throw new Exception($"Invalid {nameof(Direction2D)} given.");
        }

        public static Point2 GetCorner(this RectangleF r, DiagonalDirection2D direction)
        {
            if (direction == DiagonalDirection2D.top_left) return r.TopLeft;
            if (direction == DiagonalDirection2D.top_right) return r.TopRight();
            if (direction == DiagonalDirection2D.bottom_left) return r.BottomLeft();
            if (direction == DiagonalDirection2D.bottom_right) return r.BottomRight;
            throw new Exception($"Invalid {nameof(DiagonalDirection2D)} given.");
        }

        public static RectangleF BorderingOutside(this RectangleF r, RectangleF border, Direction2D side)
        {
            RectangleF result = r;
            if (side == Direction2D.up) return r.WithBottom(border.Top);
            if (side == Direction2D.down) return r.WithTop(border.Bottom);
            if (side == Direction2D.left) return r.WithRight(border.Left);
            if (side == Direction2D.right) return r.WithLeft(border.Right);
            throw new Exception($"Invalid {nameof(Direction2D)} given.");
        }
    }
}
