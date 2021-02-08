using Microsoft.Xna.Framework;
using MonoGame.Extended;
using DownUnder.UI.UI.Widgets.CustomEventArgs;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual {
    public sealed class DrawBackground : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.VISUAL_FUNCTION };

        protected override void Initialize() {
        }

        protected override void ConnectEvents() =>
            Parent.OnDrawBackground += Draw;

        protected override void DisconnectEvents() =>
            Parent.OnDrawBackground -= Draw;

        public override object Clone() =>
            new DrawBackground();

        void Draw(object sender, WidgetDrawArgs args) {
            if (!Parent.VisualSettings.DrawBackground)
                return;

            args.SpriteBatch.FillRectangle(
                args.DrawingArea,
                Parent.IsHighlighted
                    ? Color.Yellow
                    : Parent.VisualSettings.BackgroundColor
            );
        }
    }
}
