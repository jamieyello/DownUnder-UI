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
    public static class ConversionExtensions
    {
        public static Point ToPoint(this Point2 p)
        {
            return new Point((int)p.X, (int)p.Y);
        }

        public static Vector2 ToVector2(this Point2 p)
        {
            return new Vector2(p.X, p.Y);
        }

        public static RectangleF ToRectSize(this Vector2 v, Point2? position = null)
        {
            if (position == null)
            {
                return new RectangleF(0, 0, v.X, v.Y);
            }
            return new RectangleF(position.Value.X, position.Value.Y, v.X, v.Y);
        }
        public static RectangleF ToRectSize(this Vector2 v, float x, float y)
        {
            return new RectangleF(x, y, v.X, v.Y);
        }

        public static RectangleF ToRectSize(this Point2 p, Point2? position = null)
        {
            if (position == null)
            {
                return new RectangleF(0, 0, p.X, p.Y);
            }
            return new RectangleF(position.Value.X, position.Value.Y, p.X, p.Y);
        }
        public static RectangleF ToRectSize(this Point2 p, float x, float y)
        {
            return new RectangleF(x, y, p.X, p.Y);
        }

        public static RectangleF ToRectPosition(this Point2 p, Point2? size = null)
        {
            if (size == null)
            {
                return new RectangleF(p.X, p.Y, 0, 0);
            }
            return new RectangleF(p.X, p.Y, size.Value.X, size.Value.Y);
        }
        public static RectangleF ToRectPosition(this Point2 p, float width, float height)
        {
            return new RectangleF(p.X, p.Y, width, height);
        }

        public static RectangleF ToRectPosition(this Vector2 v, Point2? size = null)
        {
            if (size == null)
            {
                return new RectangleF(v.X, v.Y, 0, 0);
            }
            return new RectangleF(v.X, v.Y, size.Value.X, size.Value.Y);
        }
        public static RectangleF ToRectPosition(this Vector2 v, float width, float height)
        {
            return new RectangleF(v.X, v.Y, width, height);
        }

        public static Point ToPoint(this Size2 size2)
        {
            return new Point((int)size2.Width, (int)size2.Height);
        }

        public static Point2 ToPoint2(this Size2 s)
        {
            return new Point2(s.Width, s.Height);
        }

        public static Rectangle ToMonoRectangle(this System.Drawing.Rectangle rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static RectangleF ToMonoRectangleF(this System.Drawing.Rectangle rect)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Rectangle ToRectangle(this RectangleF r, bool round)
        {
            if (!round) return r.ToRectangle();
            return new Rectangle(r.X.Rounded(), r.Y.Rounded(), r.Width.Rounded(), r.Height.Rounded());
        }

        public static Size2 ToSize2(this Vector2 v)
        {
            return new Size2(v.X, v.Y);
        }

        public static Point2 Inverted(this Point2 p)
        {
            return new Point2(-p.X, -p.Y);
        }

        public static Point2 Size(this RenderTarget2D r)
        {
            return new Point2(r.Width, r.Height);
        }

        public static Point2 WithX(this Point2 p, float x)
        {
            return new Point2(x, p.Y);
        }

        public static Point2 WithY(this Point2 p, float y)
        {
            return new Point2(p.X, y);
        }

        public static Point2 Floored(this Point2 p)
        {
            return new Point2((int)p.X, (int)p.Y);
        }

        public static Point2 Rounded(this Point2 p)
        {
            return new Point2((int)(p.X + 0.5f), (int)(p.Y + 0.5f));
        }

        public static Point2 WithMinValue(this Point2 p, float v)
        {
            return new Point2(MathHelper.Max(p.X, v), MathHelper.Max(p.Y, v));
        }

        public static Point2 WithMaxValue(this Point2 p, float v)
        {
            return new Point2(MathHelper.Min(p.X, v), MathHelper.Min(p.Y, v));
        }

        /// <summary> Returns an <see cref="int"/> rounded to the nearest integer value. </summary>
        public static int Rounded(this float f)
        {
            return (int)(f + 0.5f);
        }

        /// <summary> Returns a new <see cref="Rectangle"/> without the position values. </summary>
        public static Rectangle SizeOnly(this Rectangle r)
        {
            return new Rectangle(new Point(), r.Size);
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

        /// <summary> Returns this <see cref="Point2"/> as a new <see cref="RectangleF"/>'s size. </summary>
        public static RectangleF AsRectangleSize(this Point2 p)
        {
            return new RectangleF(new Point2(), p);
        }

        public static System.Drawing.Size ToSystemSize(this Point p)
        {
            return new System.Drawing.Size(p.X, p.Y);
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

        /// <summary> Returns a <see cref="Vector2"/> with both the X and Y values rounded down. </summary>
        public static Vector2 Floored(this Vector2 v)
        {
            return new Vector2((int)v.X, (int)v.Y);
        }
    }
}
