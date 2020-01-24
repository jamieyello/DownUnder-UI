using DownUnder.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

// Todo: Duplicate RectanglF extensions for Rectangle
// Todo: Turn some extensions to properties when C# 8 arrives

namespace DownUnder
{
    public static class Extensions
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

        /// <summary>
        /// Returns true if this Point2's X or Y field is larger than the given one's.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool IsLargerThan(this Point2 p, Point2 p2)
        {
            return (p.X > p2.X || p.Y > p2.Y);
        }

        /// <summary>
        /// Returns a new Point2 with the highest X and Y values of both given Points.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point2 Max(this Point2 p, Point2 p2)
        {
            return new Point2(MathHelper.Max(p.X, p2.X), MathHelper.Max(p.Y, p2.Y));
        }

        /// <summary>
        /// Returns an integer rounded to the nearest integer value.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static int Rounded(this float f)
        {
            return (int)(f + 0.5f);
        }

        /// <summary>
        /// Returns a new Rectangle without the position values.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Rectangle SizeOnly(this Rectangle r)
        {
            return new Rectangle(new Point(), r.Size);
        }

        /// <summary>
        /// Returns a new RectangleF without the position values.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
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

        public static RectangleF AsRectangleSize(this Point2 p)
        {
            return new RectangleF(new Point2(), p);
        }

        public static System.Drawing.Size ToSystemSize(this Point p)
        {
            return new System.Drawing.Size(p.X, p.Y);
        }

        /// <summary>
        /// Returns a new RectangleF that's been resized by X pixels. (On both sides)
        /// </summary>
        /// <param name="r"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static RectangleF Resized(this RectangleF r, float x, float y)
        {
            RectangleF result = r;
            result.X -= x;
            result.Y -= y;
            result.Width += x * 2;
            result.Height += y * 2;
            return result;
        }

        /// <summary>
        /// Returns a Vector2 with both the X and Y values rounded down.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 Floored(this Vector2 v)
        {
            return new Vector2((int)v.X, (int)v.Y);
        }

        /// <summary>
        /// Returns new Color with brightness shifted by a percentage.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static Color ShiftBrightness(this Color c, float percentage)
        {
            if (percentage > 1)
            {
                return new Color
                    (
                    (byte)(c.R + (255 - c.R) * (percentage - 1)),
                    (byte)(c.G + (255 - c.G) * (percentage - 1)),
                    (byte)(c.B + (255 - c.B) * (percentage - 1)),
                    c.A
                    );
            }
            else
            {
                return new Color
                    (
                    (byte)(c.R * percentage),
                    (byte)(c.G * percentage),
                    (byte)(c.B * percentage),
                    c.A
                    );
            }
        }

        /// <summary>
        /// Returns a RectangleF to represent each line of text.
        /// </summary>
        /// <param name="sprite_font"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<RectangleF> MeasureStringAreas(this SpriteFont sprite_font, string text)
        {
            return MeasureTrimmedString(sprite_font, text, 0, 0);
        }

        /// <summary>
        /// Returns a RectangleF to represent each line of text for a selected substring of the text.
        /// </summary>
        /// <param name="sprite_font"></param>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static List<RectangleF> MeasureSubStringAreas(this SpriteFont sprite_font, string text, int index, int length, bool debug = false)
        {
            return MeasureTrimmedString(sprite_font, text, index, text.Length - length - index, debug);
        }

        /// <summary>
        /// Returns a RectangleF to represent each line of text for a trimmed version of the text.
        /// </summary>
        /// <param name="sprite_font"></param>
        /// <param name="text"></param>
        /// <param name="ltrim">How many characters should be removed from the start of the string.</param>
        /// <param name="rtrim">How many characters should be removed from the end of the string.</param>
        /// <returns></returns>
        public static List<RectangleF> MeasureTrimmedString(this SpriteFont sprite_font, string text, int ltrim, int rtrim, bool debug = false)
        {
            List<RectangleF> result = new List<RectangleF>();

            RectangleF area = new RectangleF();
            float y = 0f;
            int length_processed = 0;
            bool initial = true;
            foreach (string line in text.Split('\n').ToList())
            {
                if (length_processed + line.Length >= ltrim)
                {
                    // If this is the line where the start_trim is
                    if (initial && ltrim != 0)
                    {
                        initial = false;
                        area = MeasureSingleLineSubString(sprite_font, line, ltrim - length_processed, line.Length - (ltrim - length_processed));
                        area.Y = y;
                    }
                    else
                    {
                        area.X = 0f;
                        area.Size = sprite_font.MeasureString(line);
                    }
                    
                    // If this is the line where the end trim is
                    if (line.Length + length_processed >= text.Length - rtrim)
                    {
                        area.Width -= sprite_font.MeasureString(text.Substring(text.Length - rtrim).Split(new char[] { '\n' })[0]).X;
                        result.Add(area);
                        break;
                    }
                    
                    result.Add(area);
                }
                y += sprite_font.MeasureString("|").Y;
                area.Y = y;
                length_processed += line.Length + 1; // Add one to account for the removed \n
            }
            
            return result;
        }


        /// <summary>
        /// Returns the area of a single substring. A method used above. 
        /// </summary>
        /// <param name="sprite_font"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static RectangleF MeasureSingleLineSubString(SpriteFont sprite_font, string line, int index, int length)
        {
            return new RectangleF(
                sprite_font.MeasureString(line.Substring(0, index)).X, 
                0f, 
                sprite_font.MeasureString(line.Substring(index, length)).X, 
                sprite_font.MeasureString("|").Y
                );
        }

        /// <summary>
        /// Returns the character index in a spacial point in a string.
        /// </summary>
        /// <param name="sprite_font"></param>
        /// <param name="text"></param>
        /// <param name="point"></param>
        /// <param name="round_up">Set to true if you want to select the nearest space in between chars.</param>
        /// <returns></returns>
        public static int IndexFromPoint(this SpriteFont sprite_font, string text, Point2 point, bool round_up = false)
        {
            // Calculate the line the char is on
            float line_height = sprite_font.MeasureString("|").Y;
            List<string> lines = text.Split('\n').ToList();
            int line_y;
            if (point.Y < 0)
            {
                line_y = 0;
            }
            else
            {
                line_y = (int)(point.Y / line_height);
                if (line_y >= lines.Count) line_y = lines.Count - 1;
            }

            // Calculate the index of the line the char is on
            int line_x = 0;
            float counted_length = 0f;
            float char_length;
            foreach (char c in lines[line_y])
            {
                char_length = sprite_font.MeasureString(c.ToString()).X;
                counted_length += char_length;
                if (round_up)
                {
                    if (point.X + char_length / 2 < counted_length) break;
                }
                else
                {
                    if (point.X + char_length < counted_length) break;
                }
                
                line_x++;
            }

            // Calculate the index given the line_x and the line_y
            int result = 0;
            for (int i = 0; i < line_y; i++)
            {
                result += lines[i].Length + 1; // Add one for the removed /n
            }
            result += line_x;

            return result;
        }

        /// <summary>
        /// Get the position of a character in a string.
        /// </summary>
        /// <param name="sprite_font"></param>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static Vector2 GetCharacterPosition(this SpriteFont sprite_font, string text, int index)
        {
            return MeasureTrimmedString(sprite_font, text, index, 0)[0].Position;
        }

        /// <summary>
        /// Converts Monogame Texture2D to System.Drawing.Image.
        /// </summary>
        /// <param name="texture">Texture2D to convert.</param>
        /// <returns>Converted Image.</returns>
        public static System.Drawing.Image ToImage(this Texture2D texture)
        {
            if (texture == null) { return null; }

            byte[] textureData = new byte[4 * texture.Width * texture.Height];
            texture.GetData<byte>(textureData);

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(texture.Width, texture.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, texture.Width, texture.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            IntPtr safePtr = bmpData.Scan0;

            System.Runtime.InteropServices.Marshal.Copy(textureData, 0, safePtr, textureData.Length);
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        /// <summary>
        /// Calculates the distance between a point and a rectangle.
        /// </summary>
        public static double DistanceFrom(this RectangleF rectangle, Point2 point)
        {
            var dx = Math.Max(Math.Max(rectangle.X - point.X, point.X - rectangle.Right), 0);
            var dy = Math.Max(Math.Max(rectangle.Top - point.Y, point.Y - rectangle.Bottom), 0);
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Returns new Point2 with the sum of each.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point2 AddPoint2(this Point2 p1, Point2 p2)
        {
            return new Point2(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static RectangleF SnapInsideRectangle(this RectangleF inner, RectangleF outer, DiagonalDirections2D snapping_policy)
        {
            inner.Position = outer.Position.AddPoint2(inner.Position);

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

        public enum NumericRelationship
        {
            GreaterThan = 1,
            EqualTo = 0,
            LessThan = -1
        };

        public static NumericRelationship Compare(this ValueType value1, ValueType value2)
        {
            if (!IsNumeric(value1))
                throw new ArgumentException("value1 is not a number.");
            else if (!IsNumeric(value2))
                throw new ArgumentException("value2 is not a number.");

            // Use BigInteger as common integral type
            if (IsInteger(value1) && IsInteger(value2))
            {
                System.Numerics.BigInteger bigint1 = (System.Numerics.BigInteger)value1;
                System.Numerics.BigInteger bigint2 = (System.Numerics.BigInteger)value2;
                return (NumericRelationship)System.Numerics.BigInteger.Compare(bigint1, bigint2);
            }
            // At least one value is floating point; use Double.
            else
            {
                Double dbl1 = 0;
                Double dbl2 = 0;
                try
                {
                    dbl1 = Convert.ToDouble(value1);
                }
                catch (OverflowException)
                {
                    Console.WriteLine("value1 is outside the range of a Double.");
                }
                try
                {
                    dbl2 = Convert.ToDouble(value2);
                }
                catch (OverflowException)
                {
                    Console.WriteLine("value2 is outside the range of a Double.");
                }
                return (NumericRelationship)dbl1.CompareTo(dbl2);
            }
        }

        public static bool IsInteger(this ValueType value)
        {
            return (value is SByte || value is Int16 || value is Int32
                    || value is Int64 || value is Byte || value is UInt16
                    || value is UInt32 || value is UInt64
                    || value is System.Numerics.BigInteger);
        }

        public static bool IsNumeric(this ValueType value)
        {
            return (value is Byte ||
                    value is Int16 ||
                    value is Int32 ||
                    value is Int64 ||
                    value is SByte ||
                    value is UInt16 ||
                    value is UInt32 ||
                    value is UInt64 ||
                    value is System.Numerics.BigInteger ||
                    value is Decimal ||
                    value is Double ||
                    value is Single);
        }

        public static bool IsFloat(this ValueType value)
        {
            return (value is float | value is double | value is Decimal);
        }

        private static HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(int),  typeof(double),  typeof(decimal),
            typeof(long), typeof(short),   typeof(sbyte),
            typeof(byte), typeof(ulong),   typeof(ushort),
            typeof(uint), typeof(float)
        };

        public static bool IsNumeric(this Type type)
        {
            return NumericTypes.Contains(type) ||
                   NumericTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        private static HashSet<Type> NonIntegralTypes = new HashSet<Type>
        {
            typeof(float),  typeof(double),  typeof(decimal)
        };

        internal static bool IsIntegral(this Type type)
        {
            return !(NonIntegralTypes.Contains(type) || NonIntegralTypes.Contains(Nullable.GetUnderlyingType(type)));
        }
    }
}