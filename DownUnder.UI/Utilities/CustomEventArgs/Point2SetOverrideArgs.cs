using MonoGame.Extended;

namespace DownUnder.UI {
    public sealed class Point2SetOverrideArgs : Point2SetArgs {
        public Point2? Override { get; } = null;

        public Point2SetOverrideArgs(Point2 previous_point)
        : base(previous_point) {
        }
    }
}
