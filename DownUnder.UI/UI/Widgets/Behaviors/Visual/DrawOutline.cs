using System.Runtime.Serialization;
using MonoGame.Extended;
using DownUnder.UI.UI.Widgets.CustomEventArgs;
using DownUnder.UI.Utilities;
using Microsoft.Xna.Framework.Graphics;
using DownUnder.UI.Utilities.Extensions;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual {
    public sealed class DrawOutline : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.VISUAL_FUNCTION };
        /// <summary> How thick the outline should be. 1 by default. </summary>
        [DataMember] public float OutlineThickness { get; set; } = 1f;
        /// <summary> If true this will dim the color if <see cref="Widget.IsActive"/> is false. true by default. </summary>
        [DataMember] public bool RespectActivation { get; set; } = true;

        protected override void Initialize() {
        }

        protected override void ConnectEvents() =>
            Parent.OnDraw += Draw;

        protected override void DisconnectEvents() =>
            Parent.OnDraw -= Draw;

        public override object Clone() {
            var c = new DrawOutline();
            c.OutlineThickness = OutlineThickness;
            c.RespectActivation = RespectActivation;
            return c;
        }

        void Draw(object sender, WidgetDrawArgs args) {
            if (!Parent.VisualSettings.DrawOutline)
                return;

            DrawingTools.DrawBorder(
                DWindow.WhiteDotTexture,
                args.SpriteBatch,
                args.DrawingArea.ToRectangle(),
                OutlineThickness,
                RespectActivation && !Parent.IsActive 
                    ? Parent.VisualSettings.OutlineColor.ShiftBrightness(0.5f)
                    : Parent.VisualSettings.OutlineColor,
                Parent.VisualSettings.OutlineSides
            );
        }
    }
}