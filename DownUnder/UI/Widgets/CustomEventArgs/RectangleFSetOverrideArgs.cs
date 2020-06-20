using MonoGame.Extended;

namespace DownUnder.UI.Widgets
{
    public class RectangleFSetOverrideArgs : RectangleFSetArgs
    {
        public RectangleFSetOverrideArgs(RectangleF previous_area) : base(previous_area) { }

        public RectangleF? Override = null;
    }
}
