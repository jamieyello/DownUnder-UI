using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static Point ToPoint(this Size2 size2)
        {
            return new Point((int)size2.Width, (int)size2.Height);
        }

        public static Rectangle ToMonoRectangle(this System.Drawing.Rectangle rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Size2 ToSize2(this Vector2 v)
        {
            return new Size2(v.X, v.Y);
        }

        public static Point2 Max(this Point2 p, Point2 p2)
        {
            return new Point2(MathHelper.Max(p.X, p2.X), MathHelper.Max(p.Y, p2.Y));
        }

        public static int Rounded(this float f)
        {
            return (int)(f + 0.5f);
        }

        public static Rectangle SizeOnly(this Rectangle r)
        {
            return new Rectangle(new Point(), r.Size);
        }

        public static RectangleF SizeOnly(this RectangleF r)
        {
            return new RectangleF(new Point2(), r.Size);
        }

        public static System.Drawing.Size ToSystemSize(this Point p)
        {
            return new System.Drawing.Size(p.X, p.Y);
        }

        public static Vector2 Floored(this Vector2 v)
        {
            return new Vector2((int)v.X, (int)v.Y);
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
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static List<RectangleF> MeasureSubStringAreas(this SpriteFont sprite_font, string text, int position, int length)
        {
            return MeasureTrimmedString(sprite_font, text, position, text.Length - length);
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
            List<string> lines = text.Split('\n').ToList();

            List<RectangleF> result = new List<RectangleF>();

            RectangleF area = new RectangleF();
            float y = 0f;
            int length_processed = 0;
            bool initial = true;
            foreach (string line in lines)
            {
                if (length_processed + line.Length >= ltrim)
                {
                    // If this is the line where the start_trim is
                    if (initial && ltrim != 0)
                    {
                        initial = false;
                        area = MeasureSubString(sprite_font, line, ltrim - length_processed, line.Length - (ltrim - length_processed));
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
                        area.Width -= sprite_font.MeasureString(line.Substring(line.Length - rtrim)).X;

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
        public static RectangleF MeasureSubString(this SpriteFont sprite_font, string line, int index, int length)
        {
            string cut_text = line.Substring(0, index);
            string uncut_text = line.Substring(index, length);
            float cut_length = sprite_font.MeasureString(cut_text).X;
            float uncut_length = sprite_font.MeasureString(uncut_text).X;
            float text_height = sprite_font.MeasureString("|").Y;
            RectangleF result = new RectangleF(cut_length, 0f, uncut_length, text_height);
            return result;
        }

        public static Vector2 GetCharacterPosition(this SpriteFont sprite_font, string text, int index, bool debug = false)
        {
            List<RectangleF> area = MeasureTrimmedString(sprite_font, text, index, 0, debug);
            return area[0].Position;
        }
            
    }
}