using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.CustomEventArgs {
    public sealed class RectangleFSetOverrideArgs : RectangleFSetArgs {
        public RectangleF? Override { get; set; }

        public RectangleFSetOverrideArgs(
            RectangleF previous_area
        ) : base(previous_area) {
        }
    }
}
