using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets.Behaviors.Visual
{
    public class DrawCenteredImage : WidgetBehavior
    {
        string image;
        Texture2D texture;
        float side_spacing;

        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.VISUAL_FUNCTION };

        public DrawCenteredImage(string image, float side_spacing)
        {
            this.image = image;
            this.side_spacing = side_spacing;
        }

        protected override void Initialize()
        {
            if (Parent.ParentWindow != null) Load(this, EventArgs.Empty);
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
            Parent.MinimumSize = texture.Bounds.ToRectangleF().ResizedBy(side_spacing).Size;
        }

        private void Draw(object sender, WidgetDrawArgs args)
        {
            Rectangle draw_area = texture.Bounds.ToRectangleF().WithCenter(args.DrawingArea).ToRectangle();

            Parent.SpriteBatch.Draw(texture, draw_area, Color.White);
        }
    }
}
