using Microsoft.Xna.Framework;
using System;
using MonoGame.Extended;
using DownUnder.Utility;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class StartDragAnimation : WidgetBehavior
    {
        private ChangingValue<RectangleF> rect = new ChangingValue<RectangleF>();
        private ChangingValue<float> round_amount = new ChangingValue<float>(0f);
        private ChangingValue<Color> rect_color = new ChangingValue<Color>();
        private bool drag_rect_to_mouse = false;
        private bool snap_rect_to_mouse = false;
        private float end_circumference = 20f;
        
        public StartDragAnimation()
        {
            round_amount.Interpolation = InterpolationType.fake_sin;
            round_amount.TransitionSpeed = 1f;
            rect.Interpolation = InterpolationType.linear;
            rect.TransitionSpeed = 20f;
            rect_color.TransitionSpeed = 4f;
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

        private void StartAnimation(object sender, EventArgs args)
        {
            drag_rect_to_mouse = true;
            rect_color.SetTargetValue(new Color(0, 0, 0, 0), true);
            rect_color.TransitionSpeed = 4f;
            rect_color.SetTargetValue(Parent.Theme.OutlineColor.CurrentColor);
            rect.SetTargetValue(Parent.DrawingArea, true);
            round_amount.SetTargetValue(1f);
        }

        private void EndAnimation(object sender, EventArgs args)
        {
            drag_rect_to_mouse = false;
            snap_rect_to_mouse = false;
            round_amount.SetTargetValue(0f);
            rect_color.TransitionSpeed = 1f;
            rect_color.SetTargetValue(new Color(0, 0, 0, 0), false);
            rect.SetTargetValue(rect.GetCurrent().Resized(2f).WithCenter(rect.GetCurrent()), false);
        }

        private void Update(object sender, EventArgs args)
        {
            if (drag_rect_to_mouse)
            {
                rect.SetTargetValue(new RectangleF(0, 0, end_circumference, end_circumference).WithCenter(Parent.UpdateData.UIInputState.CursorPosition), snap_rect_to_mouse);
                if (!rect.IsTransitioning) { snap_rect_to_mouse = true; }
            }
            rect.Update(Parent.UpdateData.GameTime.GetElapsedSeconds());
            rect_color.Update(Parent.UpdateData.GameTime.GetElapsedSeconds());
            round_amount.Update(Parent.UpdateData.GameTime.GetElapsedSeconds());
        }

        private void DrawRect(object sender, EventArgs args)
        {
            RectangleF r = rect.GetCurrent();
            Parent.SpriteBatch.DrawRoundedRect(r, Math.Min(r.Width, r.Height) * 0.5f * round_amount.GetCurrent(), rect_color.GetCurrent(), 4f);
        }
    }
}
