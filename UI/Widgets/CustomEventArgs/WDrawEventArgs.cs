using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets
{
    public class WDrawEventArgs : EventArgs
    {
        public WDrawEventArgs(RectangleF drawing_area, SpriteBatch sprite_batch, Point2 cursor_position)
        {
            DrawingArea = drawing_area;
            SpriteBatch = sprite_batch;
            CursorPosition = cursor_position;
        }

        public readonly RectangleF DrawingArea;
        public readonly Point2 CursorPosition;
        public readonly SpriteBatch SpriteBatch;
    }
}
