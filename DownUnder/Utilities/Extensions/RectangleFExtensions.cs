using DownUnder.Utility;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder
{
    public static class RectangleFExtensions
    {
        public static Rectangle ToRectangle(this RectangleF r, bool round)
        {
            if (!round) return r.ToRectangle();
            return new Rectangle(r.X.Rounded(), r.Y.Rounded(), r.Width.Rounded(), r.Height.Rounded());
        }

        public static RectangleF SnapInsideRectangle(this RectangleF inner, RectangleF outer, DiagonalDirections2D snapping_policy)
        {
            inner.Position = outer.Position.WithOffset(inner.Position);

            Directions2D snapping = snapping_policy.ToPerpendicular();

            // left
            if (snapping.Left && !snapping.Right)
            {
                inner.X = outer.X;
            }

            // right
            if (!snapping.Left && snapping.Right)
            {
                inner.X = outer.X + outer.Width - inner.Width;
            }

            // left and right
            if (snapping.Left && snapping.Right)
            {
                inner.X = outer.X;
                inner.Width = outer.Width;
            }

            // up
            if (snapping.Up && !snapping.Down)
            {
                inner.Y = outer.Y;
            }

            // down
            if (!snapping.Up && snapping.Down)
            {
                inner.Y = outer.Y + outer.Height - inner.Height;
            }

            // up and down
            if (snapping.Up && snapping.Down)
            {
                inner.Y = outer.Y;
                inner.Height = outer.Height;
            }

            return inner;
        }

        public static RectangleF WithOffset(this RectangleF r, Point2 p)
        {
            RectangleF result = r;
            result.Offset(p);
            return result;
        }

        public static RectangleF WithCenter(this RectangleF r, Point2 center)
        {
            RectangleF result = r;
            result.Position = center.WithOffset(
                result.Size.ToPoint2().MultipliedBy(-0.5f)
                );
            return result;
        }

        public static RectangleF WithCenter(this RectangleF r, RectangleF r2)
        {
            return r.WithCenter(r2.Center);
        }

        /// <summary> Returns a new <see cref="RectangleF"/> positioned inside a given <see cref="RectangleF"/> with aspect ratio preserved. </summary>
        public static RectangleF FittedIn(this RectangleF r, RectangleF r2, float min_spacing = 0f)
        {
            float max_size = Math.Min(
                    Math.Min(r.Width, r2.Width - min_spacing),
                    Math.Min(r.Height, r2.Height - min_spacing));

            return new RectangleF(0f, 0f,
                    r.Width * (max_size / r.Width),
                    r.Height * (max_size / r.Height))
                    .WithCenter(r2);
        }

        /// <summary> Calculates the distance between a point and a rectangle. </summary>
        public static double DistanceFrom(this RectangleF rectangle, Point2 point)
        {
            if (rectangle.Contains(point)) return 0;
            var dx = Math.Max(Math.Max(rectangle.X - point.X, point.X - rectangle.Right), 0);
            var dy = Math.Max(Math.Max(rectangle.Top - point.Y, point.Y - rectangle.Bottom), 0);
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary> Returns a new <see cref="RectangleF"/> that's been resized by X pixels. (On all sides) </summary>
        public static RectangleF ResizedBy(this RectangleF r, float x, float y)
        {
            RectangleF result = r;
            result.X -= x;
            result.Y -= y;
            result.Width += x * 2;
            result.Height += y * 2;
            return result;
        }

        public static RectangleF ResizedBy(this RectangleF r, float amount, Directions2D directions)
        {
            RectangleF result = r;
            if (directions & Directions2D.R) { result.Width += amount; }
            if (directions & Directions2D.D) { result.Height += amount; }
            if (directions & Directions2D.U)
            {
                result.Y -= amount;
                result.Height += amount;
            }
            if (directions & Directions2D.L)
            {
                result.X -= amount;
                result.Width += amount;
            }

            return result;
        }

        /// <summary> Returns a new <see cref="RectangleF"/> that's had its size multiplied by the given modifier. </summary>
        public static RectangleF Resized(this RectangleF r, Point2 modifier)
        {
            return new RectangleF(r.Position, r.Size.ToPoint2().MultipliedBy(modifier));
        }

        /// <summary> Returns a new <see cref="RectangleF"/> that's had its size multiplied by the given modifier. </summary>
        public static RectangleF Resized(this RectangleF r, float modifier)
        {
            return new RectangleF(r.Position, r.Size.ToPoint2().MultipliedBy(modifier));
        }

        /// <summary> Returns a new RectangleF without the position values. </summary>
        public static RectangleF SizeOnly(this RectangleF r)
        {
            return new RectangleF(new Point2(), r.Size);
        }

        public static RectangleF WithX(this RectangleF r, float x)
        {
            return new RectangleF(x, r.Y, r.Width, r.Height);
        }

        public static RectangleF WithY(this RectangleF r, float y)
        {
            return new RectangleF(r.X, y, r.Width, r.Height);
        }

        public static RectangleF WithWidth(this RectangleF r, float width)
        {
            return new RectangleF(r.X, r.Y, width, r.Height);
        }

        public static RectangleF WithHeight(this RectangleF r, float height)
        {
            return new RectangleF(r.X, r.Y, r.Width, height);
        }

        public static RectangleF WithPosition(this RectangleF r, Point2 p)
        {
            return new RectangleF(p, r.Size);
        }

        public static RectangleF WithSize(this RectangleF r, Size2 s)
        {
            return new RectangleF(r.Position, s);
        }

        public static RectangleF WithSize(this RectangleF r, float width, float height)
        {
            return new RectangleF(r.X, r.Y, width, height);
        }

        public static RectangleF WithMinimumSize(this RectangleF r, Point2 s)
        {
            RectangleF result = r;
            if (r.Width < s.X) result.Width = s.X;
            if (r.Height < s.Y) result.Height = s.Y;
            return result;
        }

        public static RectangleF WithMaximumSize(this RectangleF r, Point2 s)
        {
            RectangleF result = r;
            if (r.Width > s.X) result.Width = s.X;
            if (r.Height > s.Y) result.Height = s.Y;
            return result;
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

        public static Directions2D GetCursorHoverOnBorders(this RectangleF r, Point2 p, float border_thickness)
        {
            RectangleF[] areas = GetBorderArea(r, border_thickness);
            Directions2D result = new Directions2D();

            if (areas[0].Contains(p)) result.Up = true;
            if (areas[1].Contains(p)) result.Right = true;
            if (areas[2].Contains(p)) result.Down = true;
            if (areas[3].Contains(p)) result.Left = true;

            return result;
        }

        public static RectangleF[] GetBorderArea(this RectangleF r, float thickness)
        {
            RectangleF[] areas = new RectangleF[4];
            float half_thickness = thickness / 2;

            // Top
            areas[0] = new RectangleF(r.X - half_thickness, r.Y - half_thickness, r.Width + thickness, thickness);

            // Right
            areas[1] = new RectangleF(r.X + r.Width - half_thickness, r.Y - half_thickness, thickness, r.Height + thickness);

            // Bottom
            areas[2] = new RectangleF(r.X - half_thickness, r.Y + r.Height - half_thickness, r.Width + thickness, thickness);

            //Left
            areas[3] = new RectangleF(r.X - half_thickness, r.Y - half_thickness, thickness, r.Height + thickness);

            return areas;
        }

        /// <summary>
        /// Performs linear interpolation of <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <param name="progress">Value between 0f and 1f to represent the progress between the two <see cref="RectangleF"/>s</param>
        /// <returns></returns>
        public static RectangleF Lerp(this RectangleF r1, RectangleF r2, float progress)
        {
            float inverse_progress = 1 - progress;
            return new RectangleF
                (
                r1.X * inverse_progress + r2.X * progress,
                r1.Y * inverse_progress + r2.Y * progress,
                r1.Width * inverse_progress + r2.Width * progress,
                r1.Height * inverse_progress + r2.Height * progress
                );
        }

        public static RectangleF Rounded(this RectangleF r, float accuracy)
        {
            return new RectangleF(
                ((int)((r.X + accuracy / 2) / accuracy) * accuracy),
                ((int)((r.Y + accuracy / 2) / accuracy) * accuracy),
                ((int)((r.Width + accuracy / 2) / accuracy) * accuracy),
                ((int)((r.Height + accuracy / 2) / accuracy) * accuracy)
                );
        }
    }
}
