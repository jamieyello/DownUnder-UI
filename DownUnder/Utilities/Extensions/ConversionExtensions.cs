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
        
        /// <summary> Returns this <see cref="Point2"/> as a new <see cref="RectangleF"/>'s size. </summary>
        public static RectangleF AsRectangleSize(this Point2 p)
        {
            return new RectangleF(new Point2(), p);
        }

        public static System.Drawing.Size ToSystemSize(this Point p)
        {
            return new System.Drawing.Size(p.X, p.Y);
        }

        /// <summary> Returns a <see cref="Vector2"/> with both the X and Y values rounded down. </summary>
        public static Vector2 Floored(this Vector2 v)
        {
            return new Vector2((int)v.X, (int)v.Y);
        }
    }
}
