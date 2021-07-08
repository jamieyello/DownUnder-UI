using System;
using MonoGame.Extended;

namespace DownUnder.UI {
    public class Point2SetArgs : EventArgs {
        public Point2 PreviousPoint2 { get; }

        public Point2SetArgs(Point2 previous_point) =>
            PreviousPoint2 = previous_point;
    }
}
