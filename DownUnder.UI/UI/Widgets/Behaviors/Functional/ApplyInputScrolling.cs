using System;
using Microsoft.Xna.Framework;

namespace DownUnder.UI.UI.Widgets.Behaviors.Functional {
    public sealed class ApplyInputScrolling : WidgetBehavior, ISubWidgetBehavior<ScrollBase> {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.SCROLL_FUNCTION };

        public ScrollBase BaseBehavior => Parent.Behaviors.Get<ScrollBase>();

        protected override void Initialize() {
        }

        protected override void ConnectEvents() =>
            BaseBehavior.OnUpdate += Update;

        protected override void DisconnectEvents() =>
            BaseBehavior.OnUpdate -= Update;

        public override object Clone() =>
            new ApplyInputScrolling();

        void Update(object sender, EventArgs args) {
            var scroll_base = (ScrollBase)sender;

            var scroll = Parent.UpdateData.UIInputState.Scroll;
            if (scroll == Vector2.Zero)
                return;

            var focus = Parent.ParentDWindow.ScrollableWidgetFocus;

            if (scroll.Y > 0f && focus.Up.Primary == Parent) scroll_base.AddOffset(scroll.YOnly());
            if (scroll.Y < 0f && focus.Down.Primary == Parent) scroll_base.AddOffset(scroll.YOnly());
            if (scroll.X > 0f && focus.Left.Primary == Parent) scroll_base.AddOffset(scroll.XOnly());
            if (scroll.X < 0f && focus.Right.Primary == Parent) scroll_base.AddOffset(scroll.XOnly());
        }
    }
}
