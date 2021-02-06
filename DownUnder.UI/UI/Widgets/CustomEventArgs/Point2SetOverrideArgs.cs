using MonoGame.Extended;

namespace DownUnder.UI.Widgets
{
    public class Point2SetOverrideArgs : Point2SetArgs
    {
        public Point2SetOverrideArgs(Point2 previous_point) : base(previous_point) { }

        public Point2? Override = null;
    }
}
