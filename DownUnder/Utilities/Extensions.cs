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
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static List<RectangleF> MeasureSubStringAreas(this SpriteFont sprite_font, string text, int index, int length)
        {
            return MeasureTrimmedString(sprite_font, text, index, text.Length - length);
        }

        /// <summary>
        /// Returns a RectangleF to represent each line of text for a trimmed version of the text.
        /// </summary>
        /// <param name="sprite_font"></param>
        /// <param name="text"></param>
        /// <param name="ltrim">How many characters should be removed from the start of the string.</param>
        /// <param name="rtrim">How many characters should be removed from the end of the string.</param>
        /// <returns></returns>
        public static List<RectangleF> MeasureTrimmedString(this SpriteFont sprite_font, string text, int ltrim, int rtrim)
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
    }
}