using Microsoft.Xna.Framework.Graphics;
using System;
using MonoGame.Extended;
using Microsoft.Xna.Framework;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class ShadingBehavior : WidgetBehavior
    {
        Effect shading_effect;

        protected override void ConnectToParent()
        {
            if (Parent.IsGraphicsInitialized) InitializeEffect(this, EventArgs.Empty);
            else Parent.OnGraphicsInitialized += InitializeEffect;
            Parent.OnDrawOverlayEffects += DrawEffect;
        }

        internal override void DisconnectFromParent()
        {
            Parent.OnGraphicsInitialized -= InitializeEffect;
            Parent.OnDrawOverlayEffects -= DrawEffect;
        }

        private void InitializeEffect(object sender, EventArgs args)
        {
            shading_effect = Parent.ParentWindow.EffectCollection.BlurEffect.Clone();
        }

        private void DrawEffect(object sender, EventArgs args)
        {
            shading_effect.Parameters["ShadeColor"]?.SetValue(Color.Black.ToVector4());
            shading_effect.Parameters["Size"]?.SetValue(Parent.Size);
            shading_effect.CurrentTechnique.Passes[0].Apply();
            Parent.SpriteBatch.FillRectangle(Parent.DrawingArea, Color.Transparent);
        }

        public override object Clone()
        {
            ShadingBehavior c = new ShadingBehavior();
            c.shading_effect = shading_effect.Clone();
            return c;
        }
    }
}
