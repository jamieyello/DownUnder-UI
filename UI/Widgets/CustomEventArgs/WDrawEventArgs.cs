using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets
{
    public class WDrawEventArgs : EventArgs
    {
        public WDrawEventArgs(RectangleF drawing_area, SpriteBatch sprite_batch)
        {
            DrawingArea = drawing_area;
            SpriteBatch = sprite_batch;
        }

        public readonly RectangleF DrawingArea;
        public readonly SpriteBatch SpriteBatch;
    }
}
