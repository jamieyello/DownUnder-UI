using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets.Behaviors.Behaviors
{
    public class DrawText : WidgetBehavior
    {
        public enum TextPositioningPolicy
        {
            top_left,
            center
        }

        public string Text { get; set; } = "";
        public Point2 TextPosition { get; set; } = new Point2();
        public TextPositioningPolicy TextPositioning { get; set; } = TextPositioningPolicy.top_left;

        protected override void ConnectToParent()
        {
            Parent.OnDraw += Draw;
        }

        internal override void DisconnectFromParent()
        {
            Parent.OnDraw -= Draw;
        }

        public override object Clone() {
            DrawText c = new DrawText();
            c.Text = Text;
            c.TextPosition = TextPosition;
            c.TextPositioning = TextPositioning;
            return c;
        }

        private void Draw(object sender, EventArgs args)
        {
            Parent.SpriteBatch.DrawString(Parent.SpriteFont, Text, Parent.DrawingArea.Position.WithOffset(TextPosition).Floored(), Parent.Theme.GetText(Parent.PaletteUsage).CurrentColor);
        }
    }
}
