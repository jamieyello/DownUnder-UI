using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using DownUnder.UI.UI.Widgets.CustomEventArgs;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual {
    [DataContract]
    public sealed class DrawCenteredImage : WidgetBehavior {
        [DataMember] string image;
        Texture2D texture;

        [DataMember] public float SizeModifier { get; set; }

        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.VISUAL_FUNCTION };

        public DrawCenteredImage(string image, float scaling = 1f) {
            this.image = image;
            SizeModifier = scaling;
        }

        protected override void Initialize() {
            if (Parent.ParentDWindow == null)
                return;

            Load(this, EventArgs.Empty);
        }

        protected override void ConnectEvents() {
            Parent.OnParentWindowSet += Load;
            Parent.OnDraw += Draw;
        }

        protected override void DisconnectEvents() {
            Parent.OnParentWindowSet -= Load;
            Parent.OnDraw -= Draw;
        }

        public override object Clone() =>
            throw new NotImplementedException();

        void Load(object sender, EventArgs args) =>
            texture ??= Parent.Content.Load<Texture2D>(image);

        void Draw(object sender, WidgetDrawArgs args) {
            var draw_area = texture.Bounds.ToRectangleF().WithScaledSize(SizeModifier).WithCenter(args.DrawingArea).ToRectangle();

            Parent.SpriteBatch.Draw(texture, draw_area, Color.White);
        }
    }
}