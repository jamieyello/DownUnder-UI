using System;
using DownUnder.UI.UI.Widgets.DataTypes;
using DownUnder.UI.Utilities.CommonNamespace;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using static DownUnder.UI.Utilities.CommonNamespace.Direction2D;
using static DownUnder.UI.Utilities.CommonNamespace.DiagonalDirection2D;

namespace DownUnder.UI {
    public static class RectangleFExtensions {
        public static Rectangle ToRectangle(
            this RectangleF me,
            bool round
        ) =>
            !round
                ? me.ToRectangle()
                : new Rectangle(
                    me.X.Rounded(),
                    me.Y.Rounded(),
                    me.Width.Rounded(),
                    me.Height.Rounded()
                );

        public static RectangleF BorderingInside(
            this RectangleF inner,
            RectangleF outer,
            DiagonalDirections2D sides
        ) {
            inner.Position = outer.Position.WithOffset(inner.Position);

            var snapping = sides.ToPerpendicular();

            if (snapping.Left && !snapping.Right)
                inner.X = outer.X; // left

            if (!snapping.Left && snapping.Right)
                inner.X = outer.X + outer.Width - inner.Width; // right

            if (snapping.Left && snapping.Right) { // left and right
                inner.X = outer.X;
                inner.Width = outer.Width;
            }

            if (snapping.Up && !snapping.Down)
                inner.Y = outer.Y; // up

            if (!snapping.Up && snapping.Down)
                inner.Y = outer.Y + outer.Height - inner.Height; // down

            if (snapping.Up && snapping.Down) { // up and down
                inner.Y = outer.Y;
                inner.Height = outer.Height;
            }

            return inner;
        }

        public static RectangleF WithOffset(
            this RectangleF me,
            Point2 p
        ) {
            var result = me;
            result.Offset(p);
            return result;
        }

        public static RectangleF WithCenter(
            this RectangleF me,
            Point2 center
        ) =>
            me.WithPosition(center.WithOffset(me.Size.ToPoint2().MultipliedBy(-0.5f)));

        public static RectangleF WithCenter(
            this RectangleF me,
            RectangleF rect
        ) =>
            me.WithCenter(rect.Center);

        /// <summary> Returns a new <see cref="RectangleF"/> positioned inside a given <see cref="RectangleF"/> with aspect ratio preserved. </summary>
        public static RectangleF FittedIn(
            this RectangleF me,
            RectangleF rect,
            float min_spacing = 0f
        ) {
            var max_size = Math.Min(
                Math.Min(me.Width, rect.Width - min_spacing),
                Math.Min(me.Height, rect.Height - min_spacing)
            );

            return new RectangleF(0f, 0f,
                me.Width * (max_size / me.Width),
                me.Height * (max_size / me.Height)
            ).WithCenter(rect);
        }

        /// <summary> Calculates the distance between a point and a rectangle. </summary>
        public static double DistanceFrom(this RectangleF me, Point2 point) {
            if (me.Contains(point))
                return 0;

            var dx = Math.Max(Math.Max(me.X - point.X, point.X - me.Right), 0);
            var dy = Math.Max(Math.Max(me.Top - point.Y, point.Y - me.Bottom), 0);

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static RectangleF ResizedBy(
            this RectangleF me,
            BorderSize bs,
            Point2? minimum_size = null
        ) {
            if (minimum_size == null)
                return new RectangleF(
                    me.X - bs.Left,
                    me.Y - bs.Top,
                    me.Width + bs.Left + bs.Right,
                    me.Height + bs.Top + bs.Bottom
                );

            if (bs.Top < minimum_size.Value.Y - me.Height)
                bs = bs.SetTop(minimum_size.Value.Y - me.Height);

            if (bs.Left < minimum_size.Value.X - me.Width)
                bs = bs.SetLeft(minimum_size.Value.X - me.Width);

            return new RectangleF(
                me.X - bs.Left,
                me.Y - bs.Top,
                me.Width + bs.Left + bs.Right,
                me.Height + bs.Top + bs.Bottom
            ).WithMinimumSize(minimum_size.Value);
        }

        public static RectangleF ResizedBy(
            this RectangleF me,
            float amount,
            Directions2D? directions = null,
            Point2? minimum_size = null
        ) =>
            me.ResizedBy(
                new BorderSize(amount, directions ?? Directions2D.All),
                minimum_size
            );

        public static RectangleF ResizedBy(
            this RectangleF me,
            RectanglePart part
        ) {
            var resize = new BorderSize(
                me.Height * -(1f - part.Indents.Up),
                me.Height * -(1f - part.Indents.Down),
                me.Width * -(1f -part.Indents.Left),
                me.Width * -(1f - part.Indents.Right)
            );

            return me.ResizedBy(resize);
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
        public static RectangleF WithOffset(this RectangleF me, float x, float y) => new RectangleF(me.X + x, me.Y + y, me.Width, me.Height);
        public static RectangleF WithScaledSize(this RectangleF r, float x, float y) => new RectangleF(r.X, r.Y, r.Width * x, r.Height * y);
        /// <summary> Returns a new <see cref="RectangleF"/> that's had its size multiplied by the given modifier. </summary>
        public static RectangleF WithScaledSize(this RectangleF r, Point2 modifier) => new RectangleF(r.Position, r.Size.ToPoint2().MultipliedBy(modifier));
        /// <summary> Returns a new <see cref="RectangleF"/> that's had its size multiplied by the given modifier. </summary>
        public static RectangleF WithScaledSize(this RectangleF r, float modifier) => new RectangleF(r.Position, r.Size.ToPoint2().MultipliedBy(modifier));

        public static RectangleF WithMinimumSize(
            this RectangleF me,
            Point2 min
        ) {
            var result = me;
            if (me.Width < min.X) result.Width = min.X;
            if (me.Height < min.Y) result.Height = min.Y;
            return result;
        }

        public static RectangleF WithMaximumSize(
            this RectangleF me,
            Point2 max
        ) {
            var result = me;
            if (me.Width > max.X) result.Width = max.X;
            if (me.Height > max.Y) result.Height = max.Y;
            return result;
        }

        // temp extensions until MonoGame.Extended adds these missing properties
        public static Point2 TopRight(this RectangleF me) =>
            new Point2(me.X + me.Width, me.Y);

        public static Point2 BottomLeft(this RectangleF me) =>
            new Point2(me.X, me.Y + me.Height);

        public static Directions2D GetCursorHoverOnBorders(
            this RectangleF me,
            Point2 p,
            float border_thickness
        ) {
            var areas = GetBorderArea(me, border_thickness);
            var result = new Directions2D();

            if (areas[0].Contains(p)) result.Up = true;
            if (areas[1].Contains(p)) result.Right = true;
            if (areas[2].Contains(p)) result.Down = true;
            if (areas[3].Contains(p)) result.Left = true;

            return result;
        }

        public static RectangleF[] GetBorderArea(
            this RectangleF me,
            float thickness
        ) {
            var areas = new RectangleF[4];
            var half_thickness = thickness / 2;

            areas[0] = new RectangleF(me.X - half_thickness, me.Y - half_thickness, me.Width + thickness, thickness); // Top
            areas[1] = new RectangleF(me.X + me.Width - half_thickness, me.Y - half_thickness, thickness, me.Height + thickness); // Right
            areas[2] = new RectangleF(me.X - half_thickness, me.Y + me.Height - half_thickness, me.Width + thickness, thickness); // Bottom
            areas[3] = new RectangleF(me.X - half_thickness, me.Y - half_thickness, thickness, me.Height + thickness); //Left

            return areas;
        }

        /// <summary> Performs linear interpolation of <see cref="RectangleF"/>. </summary>
        /// <param name="progress">Value between 0f and 1f to represent the progress between the two <see cref="RectangleF"/>s</param>
        public static RectangleF Lerp(
            this RectangleF me,
            RectangleF rect,
            float progress
        ) {
            var inverse_progress = 1 - progress;

            return new RectangleF (
                me.X * inverse_progress + rect.X * progress,
                me.Y * inverse_progress + rect.Y * progress,
                me.Width * inverse_progress + rect.Width * progress,
                me.Height * inverse_progress + rect.Height * progress
            );
        }

        public static RectangleF Rounded(this RectangleF r, float accuracy) =>
            new RectangleF(
                (int)((r.X + accuracy / 2) / accuracy) * accuracy,
                (int)((r.Y + accuracy / 2) / accuracy) * accuracy,
                (int)((r.Width + accuracy / 2) / accuracy) * accuracy,
                (int)((r.Height + accuracy / 2) / accuracy) * accuracy
            );

        public static float GetSide(
            this RectangleF me,
            Direction2D direction
        ) =>
            direction switch {
                up => me.Top,
                down => me.Bottom,
                left => me.Left,
                right => me.Right,
                _ => throw new Exception($"Invalid {nameof(Direction2D)} given.")
            };

        public static Point2 GetCorner(
            this RectangleF me,
            DiagonalDirection2D direction
        ) =>
            direction switch {
                top_left => me.TopLeft,
                top_right => me.TopRight(),
                bottom_left => me.BottomLeft(),
                bottom_right => me.BottomRight,
                _ => throw new Exception($"Invalid {nameof(DiagonalDirection2D)} given.")
            };

        public static RectangleF BorderingOutside(
            this RectangleF me,
            RectangleF border,
            Direction2D side
        ) =>
            side switch {
                up => me.WithBottom(border.Top),
                down => me.WithTop(border.Bottom),
                left => me.WithRight(border.Left),
                right => me.WithLeft(border.Right),
                _ => throw new Exception($"Invalid {nameof(Direction2D)} given.")
            };
    }
}
