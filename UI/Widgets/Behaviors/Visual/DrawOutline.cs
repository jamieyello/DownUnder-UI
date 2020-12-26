using DownUnder.Utilities;
using MonoGame.Extended;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    public class DrawOutline : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.VISUAL_FUNCTION };

        /// <summary> How thick the outline should be. 1 by default. </summary>
        [DataMember]
        public float OutlineThickness { get; set; } = 1f;

        protected override void Initialize()
        {
        }

        protected override void ConnectEvents()
        {
            Parent.OnDrawOverlay += Draw;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnDrawOverlay -= Draw;
        }

        public override object Clone()
        {
            return new DrawOutline();
        }

        void Draw(object sender, WidgetDrawArgs args)
        {
            if (Parent.VisualSettings.DrawOutline)
            {
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
}
