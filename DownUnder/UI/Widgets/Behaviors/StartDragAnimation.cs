using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class StartDragAnimation : WidgetBehavior
    {
        public StartDragAnimation()
        {

        }

        protected override void AddEvents()
        {
            Parent.OnDrawNoClip += DrawRect;
            Parent.OnUpdate += Update;
            Parent.OnDrag += StartAnimation;
            Parent.OnDrop += EndAnimation;
        }

        internal override void Deconstruct()
        {
            Parent.OnDrawNoClip -= DrawRect;
            Parent.OnUpdate -= Update;
            Parent.OnDrag -= StartAnimation;
            Parent.OnDrop -= EndAnimation;
        }

        public void StartAnimation(object sender, EventArgs args)
        {

        }

        public void EndAnimation(object sender, EventArgs args)
        {
            
        }

        public void Update(object sender, EventArgs args)
        {

        }
        
        public void DrawRect(object sender, EventArgs args)
        {
            Parent.SpriteBatch.DrawString(Parent.SpriteFont, "test", new Vector2(), Color.White);
            Parent.SpriteBatch.DrawRoundedRect(new RectangleF(0, 0, 100, 100), 0.5f);
        }
    }
}
