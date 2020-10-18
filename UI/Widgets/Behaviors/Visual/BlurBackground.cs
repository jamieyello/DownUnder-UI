using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    class BlurBackground : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.COSMETIC_HIGH_PERFORMANCE };
        Effect effect;

        protected override void Initialize()
        {
            //Parent.DrawingMode = Widget.DrawingModeType.use_render_target;
            if (Parent.ParentWindow != null) LoadEffect(this, EventArgs.Empty);
        }

        protected override void ConnectEvents()
        {
            Parent.OnDrawBackground += ApplyEffect;
            Parent.OnParentWindowSet += LoadEffect;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnDrawBackground -= ApplyEffect;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        private void LoadEffect(object sender, EventArgs args)
        {
            //Parent.ParentWidget.DrawingMode = Widget.DrawingModeType.use_render_target;
            effect = Parent.ParentWindow.ParentGame.Content.Load<Effect>("DownUnder Native Content/Effects/Blur");
        }

        private void ApplyEffect(object sender, WidgetDrawArgs args)
        {
            args.RestartImmediate();
            effect.CurrentTechnique.Passes[0].Apply();
            args.SpriteBatch.Draw(args.ParentRender, args.DrawingArea.ToRectangle(), args.AreaInRender.ToRectangle(), Color.White);
            args.RestartDefault();
        }
    }
}
