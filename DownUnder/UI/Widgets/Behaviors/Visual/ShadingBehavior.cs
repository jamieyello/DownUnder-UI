using Microsoft.Xna.Framework.Graphics;
using System;
using MonoGame.Extended;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    [DataContract] public class ShadingBehavior : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.COSMETIC_HIGH_PERFORMANCE };

        Effect shading_effect;

        [DataMember] public float ShadeVisibility = 0.5f;
        [DataMember] public Color ShadeColor { get; set; } = Color.Black;

        [DataMember] public float BorderWidth = 40f;
        [DataMember] public float BorderExponential = 1f;
        [DataMember] public float BorderVisibility = 1f;

        [DataMember] public Point2 GradientVisibility = new Point2(0.5f, 0.5f);
        [DataMember] public Point2 GradientExponential = new Point2(1f, 1f);

        [DataMember] public bool UseWidgetOutlineColor = false;

        protected override void Initialize() {
            Parent.DrawingMode = Widget.DrawingModeType.use_render_target;
        }

        protected override void ConnectEvents() {
            if (Parent.IsGraphicsInitialized) InitializeEffect(this, EventArgs.Empty);
            else Parent.OnGraphicsInitialized += InitializeEffect;
            Parent.OnDrawOverlayEffects += DrawEffect;
            Parent.OnDispose += Dispose;
        }

        protected override void DisconnectEvents() {
            Parent.OnGraphicsInitialized -= InitializeEffect;
            Parent.OnDrawOverlayEffects -= DrawEffect;
            Parent.OnDispose -= Dispose;
            Dispose(this, EventArgs.Empty);
        }

        private void InitializeEffect(object sender, EventArgs args) {
            shading_effect = Parent.ParentWindow.EffectCollection.ShadingEffect.Clone();
        }

        private void DrawEffect(object sender, EventArgs args) {
            if (UseWidgetOutlineColor) shading_effect.Parameters["ShadeColor"].SetValue(Parent.Theme.GetOutline(Parent.WidgetRole).CurrentColor.ToVector4());
            else shading_effect.Parameters["ShadeColor"].SetValue(ShadeColor.ToVector4());
            shading_effect.Parameters["BorderWidth"]?.SetValue(BorderWidth);
            shading_effect.Parameters["BorderExponential"]?.SetValue(BorderExponential);
            shading_effect.Parameters["BorderVisibility"]?.SetValue(BorderVisibility);
            shading_effect.Parameters["GradientVisibility"]?.SetValue(GradientVisibility);
            shading_effect.Parameters["GradientExponential"]?.SetValue(GradientExponential);
            shading_effect.Parameters["Size"]?.SetValue(Parent.Size);
            
            shading_effect.CurrentTechnique.Passes[0].Apply();
            Parent.SpriteBatch.FillRectangle(Parent.DrawingAreaUnscrolled, Color.Transparent);
        }

        public override object Clone() {
            ShadingBehavior c = new ShadingBehavior();
            if (shading_effect != null) c.shading_effect = shading_effect.Clone();
            c.ShadeColor = ShadeColor;
            return c;
        }

        private void Dispose(object sender, EventArgs args)
        {
            shading_effect?.Dispose();
        }

        public static ShadingBehavior SubtleBlue => new ShadingBehavior()
        {
            ShadeColor = Color.DeepSkyBlue.ShiftBrightness(0.2f),
            BorderVisibility = 0.6f,
            BorderWidth = 10f,
            GradientVisibility = new Point2(0.2f, 0.2f)
        };
    }
}
