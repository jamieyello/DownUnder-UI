using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using DownUnder.Utility;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class StartDragAnimation : WidgetBehavior
    {
        private ChangingValue<float> round_amount = new ChangingValue<float>(0f);

        public StartDragAnimation()
        {
            round_amount.Interpolation = InterpolationType.fake_sin;
            round_amount.TransitionSpeed = 1f;
        }

        protected override void ConnectEvents()
        {
            Parent.OnDrawNoClip += DrawRect;
            Parent.OnUpdate += Update;
            Parent.OnDrag += StartAnimation;
            Parent.OnDrop += EndAnimation;
        }

        internal override void DisconnectEvents()
        {
            Parent.OnDrawNoClip -= DrawRect;
            Parent.OnUpdate -= Update;
            Parent.OnDrag -= StartAnimation;
            Parent.OnDrop -= EndAnimation;
        }

        public void StartAnimation(object sender, EventArgs args)
        {
            round_amount.SetTargetValue(1f);
        }

        public void EndAnimation(object sender, EventArgs args)
        {
            round_amount.SetTargetValue(0f);
        }

        public void Update(object sender, EventArgs args)
        {
            round_amount.Update(Parent.UpdateData.GameTime.GetElapsedSeconds());
        }
        
        public void DrawRect(object sender, EventArgs args)
        {
            Parent.SpriteBatch.DrawString(Parent.SpriteFont, "test", new Vector2(), Color.White);
            Parent.SpriteBatch.DrawRoundedRect(new RectangleF(10, 10, 100, 100), 50f * round_amount.GetCurrent(), Color.Black, 4f);
        }
    }
}
