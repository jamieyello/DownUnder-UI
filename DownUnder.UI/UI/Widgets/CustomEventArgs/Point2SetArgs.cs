using System;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.CustomEventArgs
{
    public class Point2SetArgs : EventArgs
    {
        public Point2SetArgs(Point2 previous_point)
        {
            PreviousPoint2 = previous_point;
        }

        public readonly Point2 PreviousPoint2;
    }
}
