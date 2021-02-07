using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.CustomEventArgs
{
    public class RectangleFSetOverrideArgs : RectangleFSetArgs
    {
        public RectangleFSetOverrideArgs(RectangleF previous_area) : base(previous_area) { }

        public RectangleF? Override = null;
    }
}
