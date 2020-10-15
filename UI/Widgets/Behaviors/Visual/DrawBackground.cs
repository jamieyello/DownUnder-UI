using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    class DrawBackground : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.VISUAL_FUNCTION };

        protected override void Initialize()
        {
        }

        protected override void ConnectEvents()
        {
            Parent.OnDrawBackground += Draw;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnDrawBackground -= Draw;
        }

        public override object Clone()
        {
            return new DrawBackground();
        }

        void Draw(object sender, WidgetDrawArgs args)
        {
            if (Parent.VisualSettings.DrawBackground)
            {
                args.SpriteBatch.FillRectangle(args.DrawingArea, Parent.IsHighlighted ? Color.Yellow : Parent.VisualSettings.BackgroundColor);
            }
        }
    }
}
