using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets
{
    public class WidgetResizeEventArgs : EventArgs
    {
        public WidgetResizeEventArgs(RectangleF previous_area) 
        {
            PreviousArea = previous_area;
        }
        
        public readonly RectangleF PreviousArea;
    }
}
