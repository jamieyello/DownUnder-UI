using System;
using DownUnder.UI.Utilities.Extensions;
using Microsoft.Xna.Framework;

namespace DownUnder.UI.UI.Widgets.Behaviors.Functional
{
    public class ApplyInputScrolling : WidgetBehavior, ISubWidgetBehavior<ScrollBase>
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.SCROLL_FUNCTION };

        public ScrollBase BaseBehavior => Parent.Behaviors.Get<ScrollBase>();

        protected override void Initialize()
        {
        }

        protected override void ConnectEvents()
        {
            BaseBehavior.OnUpdate += Update;
        }

        protected override void DisconnectEvents()
        {
            BaseBehavior.OnUpdate -= Update;
        }

        public override object Clone()
        {
            ApplyInputScrolling c = new ApplyInputScrolling();
            return c;
        }

        void Update(object sender, EventArgs args)
        {
            ScrollBase scroll_base = (ScrollBase)sender;

            if (Parent.UpdateData.UIInputState.Scroll != Vector2.Zero)
            {
                if (Parent.UpdateData.UIInputState.Scroll.Y > 0f && Parent.ParentDWindow.ScrollableWidgetFocus.Up.Primary == Parent) scroll_base.AddOffset(Parent.UpdateData.UIInputState.Scroll.YOnly());
                if (Parent.UpdateData.UIInputState.Scroll.Y < 0f && Parent.ParentDWindow.ScrollableWidgetFocus.Down.Primary == Parent) scroll_base.AddOffset(Parent.UpdateData.UIInputState.Scroll.YOnly());
                if (Parent.UpdateData.UIInputState.Scroll.X > 0f && Parent.ParentDWindow.ScrollableWidgetFocus.Left.Primary == Parent) scroll_base.AddOffset(Parent.UpdateData.UIInputState.Scroll.XOnly());
                if (Parent.UpdateData.UIInputState.Scroll.X < 0f && Parent.ParentDWindow.ScrollableWidgetFocus.Right.Primary == Parent) scroll_base.AddOffset(Parent.UpdateData.UIInputState.Scroll.XOnly());
            }
        }
    }
}
