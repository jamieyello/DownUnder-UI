using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets
{
    public class WDrawEventArgs : EventArgs
    {
        public WDrawEventArgs(RectangleF drawing_area)
        {
            DrawingArea = drawing_area;
        }

        public readonly RectangleF DrawingArea;
    }
}
