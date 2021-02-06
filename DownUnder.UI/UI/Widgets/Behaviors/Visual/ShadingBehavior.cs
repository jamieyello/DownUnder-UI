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
        [DataMember] bool _draw_as_background = true;
        [DataMember] public float ShadeVisibility = 1f;
        [DataMember] public Color ShadeColor { get; set; } = Color.Black;
        [DataMember] public float BorderWidth = 40f;
        [DataMember] public float BorderExponential = 1f;
        [DataMember] public float BorderVisibility = 1f;
        [DataMember] public Point2 GradientVisibility = new Point2(0.5f, 0.5f);
        [DataMember] public Point2 GradientExponential = new Point2(1f, 1f);
        [DataMember] public bool UseWidgetOutlineColor = false;

        public bool DrawAsBackGround
        {
            get => _draw_as_background;
            set
            {
                if (value == _draw_as_background) return;
                _draw_as_background = value;
                if (IsConnected)
                {
                    if (value)
                    {
                        Parent.OnDraw -= DrawEffect;
                        Parent.OnDrawBackground += DrawEffect;
                    }
                    else
                    {
                        Parent.OnDraw += DrawEffect;
                        Parent.OnDrawBackground -= DrawEffect;
                    }
                }
            }
        }

        protected override void Initialize() { }

        protected override void ConnectEvents() {
            if (Parent.IsGraphicsInitialized) InitializeEffect(this, EventArgs.Empty);
            else Parent.OnGraphicsInitialized += InitializeEffect;
            if (DrawAsBackGround) Parent.OnDrawBackground += DrawEffect;
            else Parent.OnDraw += DrawEffect;
            Parent.OnDispose += Dispose;
        }

        protected override void DisconnectEvents() {
            Parent.OnGraphicsInitialized -= InitializeEffect;
            if (DrawAsBackGround) Parent.OnDrawBackground -= DrawEffect;
            else Parent.OnDraw -= DrawEffect;
            Parent.OnDispose -= Dispose;
            Dispose(this, EventArgs.Empty);
        }

        private void InitializeEffect(object sender, EventArgs args) {
            shading_effect = Parent.ParentDWindow.EffectCollection.ShadingEffect.Clone();
        }

        private void DrawEffect(object sender, WidgetDrawArgs args) {
            args.RestartImmediate();
            if (UseWidgetOutlineColor) shading_effect.Parameters["ShadeColor"].SetValue(Parent.VisualSettings.OutlineColor.ToVector4());
            else shading_effect.Parameters["ShadeColor"]?.SetValue(ShadeColor.ToVector4());
            shading_effect.Parameters["BorderWidth"]?.SetValue(BorderWidth);
            shading_effect.Parameters["BorderVisibility"]?.SetValue(BorderVisibility);
            shading_effect.Parameters["ShadeVisibility"]?.SetValue(ShadeVisibility);
            shading_effect.Parameters["BorderExponential"]?.SetValue(BorderExponential);
            shading_effect.Parameters["BorderVisibility"]?.SetValue(BorderVisibility);
            shading_effect.Parameters["GradientVisibility"]?.SetValue(GradientVisibility);
            shading_effect.Parameters["GradientExponential"]?.SetValue(GradientExponential);
            shading_effect.Parameters["Size"]?.SetValue(Parent.Size);
            shading_effect.CurrentTechnique.Passes[0].Apply();
            args.SpriteBatch.FillRectangle(args.DrawingArea, Color.Transparent);
            args.RestartDefault();
        }

        public override object Clone() {
            ShadingBehavior c = new ShadingBehavior();

            c.ShadeVisibility = ShadeVisibility;
            c.ShadeColor = ShadeColor;
            c.BorderWidth = BorderWidth;
            c.BorderExponential = BorderExponential;
            c.BorderVisibility = BorderVisibility;
            c.GradientVisibility = GradientVisibility;
            c.GradientExponential = GradientExponential;
            c.UseWidgetOutlineColor = false;
            c.DrawAsBackGround = DrawAsBackGround;

            return c;
        }

        private void Dispose(object sender, EventArgs args)
        {
            shading_effect?.Dispose();
        }

        public static ShadingBehavior SubtleBlue => new ShadingBehavior()
        {
            ShadeColor = Color.DeepSkyBlue.ShiftBrightness(0.3f),
            BorderVisibility = 0.6f,
            BorderWidth = 10f,
            GradientVisibility = new Point2(0.25f, 0.25f)
        };

        public static ShadingBehavior GlowingGreen => new ShadingBehavior()
        {
            ShadeColor = Color.LimeGreen.ShiftBrightness(0.3f),
            BorderVisibility = 0.3f,
            BorderWidth = 10f,
            GradientVisibility = new Point2(0.25f, 0.25f)
        };
    }
}
