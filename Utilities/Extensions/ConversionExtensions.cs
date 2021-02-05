using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DownUnder
{
    public static class ConversionExtensions
    {
        public static Point ToPoint(this Point2 p) =>
            new Point((int)p.X, (int)p.Y);

        public static Point2 ToPoint2(this Vector2 v) =>
            new Point2(v.X, v.Y);

        public static Vector2 ToVector2(this Point2 p) =>
            new Vector2(p.X, p.Y);

        public static RectangleF ToRectSize(this Vector2 v, Point2? position = null) =>
            position == null ? new RectangleF(0, 0, v.X, v.Y) : new RectangleF(position.Value.X, position.Value.Y, v.X, v.Y);

        public static RectangleF ToRectSize(this Vector2 v, float x, float y) =>
            new RectangleF(x, y, v.X, v.Y);

        public static RectangleF ToRectSize(this Point2 p, Point2? position = null) =>
            position == null ? new RectangleF(0, 0, p.X, p.Y) : new RectangleF(position.Value.X, position.Value.Y, p.X, p.Y);

        public static RectangleF ToRectSize(this Point2 p, float x, float y) =>
            new RectangleF(x, y, p.X, p.Y);

        public static RectangleF ToRectPosition(this Point2 p, Point2? size = null) =>
            size == null ? new RectangleF(p.X, p.Y, 0, 0) : new RectangleF(p.X, p.Y, size.Value.X, size.Value.Y);

        public static RectangleF ToRectPosition(this Point2 p, float width, float height) =>
            new RectangleF(p.X, p.Y, width, height);

        public static RectangleF ToRectPosition(this Vector2 v, Point2? size = null) =>
            size == null ? new RectangleF(v.X, v.Y, 0, 0) : new RectangleF(v.X, v.Y, size.Value.X, size.Value.Y);

        public static RectangleF ToRectPosition(this Vector2 v, float width, float height) =>
            new RectangleF(v.X, v.Y, width, height);

        public static Point ToPoint(this Size2 size2) =>
            new Point((int)size2.Width, (int)size2.Height);

        public static Point2 ToPoint2(this Size2 s) =>
            new Point2(s.Width, s.Height);

        public static Rectangle ToMonoRectangle(this System.Drawing.Rectangle rect) =>
            new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

        public static RectangleF ToMonoRectangleF(this System.Drawing.Rectangle rect) =>
            new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);

        public static Size2 ToSize2(this Vector2 v) =>
            new Size2(v.X, v.Y);

        public static Point2 Inverted(this Point2 p) =>
            new Point2(-p.X, -p.Y);

        public static Point2 Size(this RenderTarget2D r) =>
            new Point2(r.Width, r.Height);

        public static Point2 WithX(this Point2 p, float x) =>
            new Point2(x, p.Y);

        public static Point2 WithY(this Point2 p, float y) =>
            new Point2(p.X, y);

        public static Point2 Floored(this Point2 p) =>
            new Point2((int)p.X, (int)p.Y);

        public static Point2 Rounded(this Point2 p) =>
            new Point2((int)(p.X + 0.5f), (int)(p.Y + 0.5f));

        public static Point2 WithMinValue(this Point2 p, float v) =>
            new Point2(MathHelper.Max(p.X, v), MathHelper.Max(p.Y, v));

        public static Point2 WithMaxValue(this Point2 p, float v) =>
            new Point2(MathHelper.Min(p.X, v), MathHelper.Min(p.Y, v));

        /// <summary> Returns an <see cref="int"/> rounded to the nearest integer value. </summary>
        public static int Rounded(this float f) =>
            (int)(f + 0.5f);

        /// <summary> Returns a new <see cref="Rectangle"/> without the position values. </summary>
        public static Rectangle SizeOnly(this Rectangle r) =>
            new Rectangle(new Point(), r.Size);

        /// <summary> Returns this <see cref="Point2"/> as a new <see cref="RectangleF"/>'s size. </summary>
        public static RectangleF AsRectangleSize(this Point2 p) =>
            new RectangleF(new Point2(), p);

        /// <summary> Returns this <see cref="Point2"/> as a new <see cref="RectangleF"/>'s size. </summary>
        public static RectangleF AsRectangleSize(this Point2 p, Point2 position) =>
            new RectangleF(position, p);

        /// <summary> Returns this <see cref="Point2"/> as a new <see cref="RectangleF"/>'s position. </summary>
        public static RectangleF AsRectanglePosition(this Point2 p, Point2 size) =>
            new RectangleF(p, size);

        public static System.Drawing.Size ToSystemSize(this Point p) =>
            new System.Drawing.Size(p.X, p.Y);

        /// <summary> Returns a <see cref="Vector2"/> with both the X and Y values rounded down. </summary>
        public static Vector2 Floored(this Vector2 v) =>
            new Vector2((int)v.X, (int)v.Y);
    }
}
