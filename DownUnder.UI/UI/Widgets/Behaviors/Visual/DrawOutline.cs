using System.Runtime.Serialization;
using MonoGame.Extended;
using DownUnder.UI.UI.Widgets.CustomEventArgs;
using DownUnder.UI.Utilities;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual {
    public sealed class DrawOutline : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.VISUAL_FUNCTION };

        /// <summary> How thick the outline should be. 1 by default. </summary>
        [DataMember]
        public float OutlineThickness { get; set; } = 1f;

        protected override void Initialize() {
        }

        protected override void ConnectEvents() =>
            Parent.OnDraw += Draw;

        protected override void DisconnectEvents() =>
            Parent.OnDraw -= Draw;

        public override object Clone() =>
            new DrawOutline();

        void Draw(object sender, WidgetDrawArgs args) {
            if (!Parent.VisualSettings.DrawOutline)
                return;

            DrawingTools.DrawBorder(
                Parent._white_dot,
                args.SpriteBatch,
                args.DrawingArea.ToRectangle(),
                OutlineThickness,
                Parent.VisualSettings.OutlineColor,
                Parent.VisualSettings.OutlineSides
            );
        }
    }
}