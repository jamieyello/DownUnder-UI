using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    [DataContract]
    public sealed class BlurBackground : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.COSMETIC_HIGH_PERFORMANCE };

        Effect effect;
        /// <summary> If true then disable <see cref="Widget.VisualSettings"/> DrawBackground. true by default. </summary>
        bool DisableDrawBackground = true;

        protected override void Initialize() {
            Parent.VisualSettings.DrawBackground = false;
            if (Parent.ParentDWindow is { })
                LoadEffect(this, EventArgs.Empty);
        }

        protected override void ConnectEvents() {
            Parent.OnDrawBackground += ApplyEffect;
            Parent.OnParentWindowSet += LoadEffect;
        }

        protected override void DisconnectEvents() {
            Parent.OnDrawBackground -= ApplyEffect;
            Parent.OnParentWindowSet -= LoadEffect;
        }

        public override object Clone()
        {
            var c = new BlurBackground();
            c.DisableDrawBackground = DisableDrawBackground;
            return c;
        }

        void LoadEffect(object sender, EventArgs args) =>
            effect ??= Parent.ParentDWindow.ParentGame.Content.Load<Effect>("DownUnder Native Content/Effects/Blur");

        void ApplyEffect(object sender, WidgetDrawArgs args) {
            var background = args.GetBackgroundRender();
            args.RestartImmediate();

            effect.CurrentTechnique.Passes[0].Apply();
            args.SpriteBatch.Draw(background, args.DrawingArea.ToRectangle(), args.AreaInRender.ToRectangle(), Color.White);
            args.RestartDefault();
        }
    }
}