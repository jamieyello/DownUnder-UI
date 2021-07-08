using MonoGame.Extended;

namespace DownUnder.UI {
    public sealed class RectangleFSetOverrideArgs : RectangleFSetArgs {
        public RectangleF? Override { get; set; }

        public RectangleFSetOverrideArgs(
            RectangleF previous_area
        ) : base(previous_area) {
        }
    }
}
