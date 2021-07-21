using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    [DataContract]
    public sealed class DrawBackground : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.VISUAL_FUNCTION };
        /// <summary> If true this will dim the color if <see cref="Widget.Enabled"/> is false. true by default. </summary>
        [DataMember] public bool RespectActivation { get; set; } = true;

        protected override void Initialize() {
        }

        protected override void ConnectEvents() =>
            Parent.OnDrawBackground += Draw;

        protected override void DisconnectEvents() =>
            Parent.OnDrawBackground -= Draw;

        public override object Clone() {
            var c = new DrawBackground();
            c.RespectActivation = RespectActivation;
            return c;
        }

        void Draw(object sender, WidgetDrawArgs args) {
            if (!Parent.VisualSettings.DrawBackground)
                return;

            args.SpriteBatch.FillRectangle(
                args.DrawingArea,
                Parent.IsHighlighted
                    ? Color.Yellow
                    : !Parent.Enabled && RespectActivation
                        ? Parent.VisualSettings.BackgroundColor.ShiftBrightness(0.5f)
                        : Parent.VisualSettings.BackgroundColor
            );
        }
    }
}
