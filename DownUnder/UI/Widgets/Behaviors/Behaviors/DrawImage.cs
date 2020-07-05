using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DownUnder.UI.Widgets.Behaviors.Behaviors
{
    public class DrawImage : WidgetBehavior
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle? DestinationRectangle;
        public Rectangle? SourceRectangle { get; set; }
        public Vector2? Origin;
        public float Rotation = 0f;
        public Vector2? Scale;
        public Color? Color;
        public SpriteEffects SpriteEffects = SpriteEffects.None;
        public float LayerDepth = 0f;
        public bool CloneImage { get; set; } = false;
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.VISUAL_FUNCTION };


        public override object Clone()
        {
            throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            
        }

        protected override void ConnectEvents()
        {
            Parent.OnDraw += Draw;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnDraw -= Draw;
        }

        private void Draw(object sender, EventArgs args)
        {

            //Parent.SpriteBatch.Draw(Texture,);
        }
    }
}
