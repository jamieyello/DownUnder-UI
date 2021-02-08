using Microsoft.Xna.Framework;

namespace DownUnder.UI.Input {
    public sealed class Cursor {
        public bool Triggered { get; set; }
        public bool HeldDown { get; set; }
        public Point Position { get; set; }

        public override string ToString() =>
            $"Triggered: {Triggered}, HeldDown: {HeldDown}, Position: {Position}";
    }
}