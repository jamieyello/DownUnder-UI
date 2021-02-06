using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets
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
