using System;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.CustomEventArgs {
    public class RectangleFSetArgs : EventArgs {
        public RectangleF PreviousArea { get; }

        public RectangleFSetArgs(RectangleF previous_area) =>
            PreviousArea = previous_area;
    }
}