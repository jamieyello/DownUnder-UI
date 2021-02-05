using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets
{
    public class RectangleFSetArgs : EventArgs
    {
        public RectangleFSetArgs(RectangleF previous_area)
        {
            PreviousArea = previous_area;
        }

        public readonly RectangleF PreviousArea;
    }
}
