using System;
using System.Runtime.Serialization;
using DownUnder.UI.UI.Widgets.CustomEventArgs;
using DownUnder.UI.Utilities.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual
{
    [DataContract]
    public class DrawCenteredImage : WidgetBehavior
    {
        [DataMember] string image;
        Texture2D texture;
        [DataMember] public float SizeModifier { get; set; }

        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.VISUAL_FUNCTION };

        public DrawCenteredImage(string image, float scaling = 1f)
        {
            this.image = image;
            SizeModifier = scaling;
        }

        protected override void Initialize()
        {
            if (Parent.ParentDWindow != null) Load(this, EventArgs.Empty);
        }

        protected override void ConnectEvents()
        {
            Parent.OnParentWindowSet += Load;
            Parent.OnDraw += Draw;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnParentWindowSet -= Load;
            Parent.OnDraw -= Draw;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        void Load(object sender, EventArgs args)
        {
            if (texture == null) texture = Parent.Content.Load<Texture2D>(image);
        }

        private void Draw(object sender, WidgetDrawArgs args)
        {
            Rectangle draw_area = texture.Bounds.ToRectangleF().WithScaledSize(SizeModifier).WithCenter(args.DrawingArea).ToRectangle();

            Parent.SpriteBatch.Draw(texture, draw_area, Color.White);
        }
    }
}
