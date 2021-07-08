using System;
using MonoGame.Extended;

namespace DownUnder.UI {
    public class RectangleFSetArgs : EventArgs {
        public RectangleF PreviousArea { get; }

        public RectangleFSetArgs(RectangleF previous_area) =>
            PreviousArea = previous_area;
    }
}