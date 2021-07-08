using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using DownUnder.UI.Utilities;

namespace DownUnder.UI.Widgets.Behaviors.Visual {
    public sealed class DragOffOutline : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.COSMETIC_MID_PERFORMANCE };

        readonly ChangingValue<RectangleF> rect = new ChangingValue<RectangleF>();
        readonly ChangingValue<float> round_amount = new ChangingValue<float>(0f);
        readonly ChangingValue<Color> rect_color = new ChangingValue<Color>();
        bool drag_rect_to_mouse;
        bool snap_rect_to_mouse;
        const float circumference = 20f;
        float _expand_rect_on_release = 1f;

        public DragOffOutline() {
            round_amount.Interpolation = InterpolationType.fake_sin;
            round_amount.TransitionSpeed = 1f;
            rect.Interpolation = InterpolationType.linear;
            rect.TransitionSpeed = 20f;
            rect_color.TransitionSpeed = 4f;
        }

        protected override void Initialize() { }

        protected override void ConnectEvents() {
            Parent.OnDrawNoClip += DrawRect;
            Parent.OnUpdate += Update;
            Parent.OnDrag += StartAnimation;
            Parent.OnDrop += EndAnimation;
        }

        protected override void DisconnectEvents() {
            Parent.OnDrawNoClip -= DrawRect;
            Parent.OnUpdate -= Update;
            Parent.OnDrag -= StartAnimation;
            Parent.OnDrop -= EndAnimation;
        }

        void StartAnimation(object sender, EventArgs args) {
            drag_rect_to_mouse = true;
            rect_color.SetTargetValue(new Color(0, 0, 0, 0), true);
            rect_color.TransitionSpeed = 4f;
            rect_color.SetTargetValue(Parent.VisualSettings.OutlineColor);
            //rect.SetTargetValue(Parent.DrawingArea, true);
            round_amount.SetTargetValue(1f);
        }

        void EndAnimation(object sender, EventArgs args) {
            drag_rect_to_mouse = false;
            snap_rect_to_mouse = false;
            round_amount.SetTargetValue(0f);
            rect_color.TransitionSpeed = 1f;
            rect_color.SetTargetValue(new Color(0, 0, 0, 0));
            rect.SetTargetValue(rect.Current.WithScaledSize(_expand_rect_on_release).WithCenter(rect.Current));
        }

        void Update(object sender, EventArgs args) {
            if (drag_rect_to_mouse) {
                var is_drop_acceptable = Parent.ParentDWindow.HoveredWidgets.Primary?.IsDropAcceptable(Parent.ParentDWindow.DraggingObject);
                var victim = Parent.ParentDWindow.HoveredWidgets.Primary;

                if (Parent.ParentDWindow.DraggingObject is Widget dragging_widget && is_drop_acceptable != null && is_drop_acceptable.Value) {
                    // Set to new Widget area
                    rect.SetTargetValue(
                        victim.DesignerObjects.GetAddWidgetArea(dragging_widget).WithOffset(victim.PositionInWindow),
                        snap_rect_to_mouse);
                    _expand_rect_on_release = 1f;
                    round_amount.SetTargetValue(.5f);
                }
                else {
                    // Set to default circle
                    rect.SetTargetValue(new RectangleF(0, 0, circumference, circumference).WithCenter(Parent.UpdateData.UIInputState.CursorPosition), snap_rect_to_mouse);
                    _expand_rect_on_release = 2f;
                    round_amount.SetTargetValue(1f);
                }
            }
            rect.Update(Parent.UpdateData.ElapsedSeconds);
            rect_color.Update(Parent.UpdateData.ElapsedSeconds);
            round_amount.Update(Parent.UpdateData.ElapsedSeconds);
        }

        void DrawRect(object sender, EventArgs args) {
            var r = rect.Current;
            var radius = Math.Min(r.Width, r.Height) * 0.5f * round_amount.Current;

            Parent.SpriteBatch.DrawRoundedRect(
                r,
                radius,
                rect_color.Current,
                4f
            );
        }

        public override object Clone() =>
            new DragOffOutline();
    }
}