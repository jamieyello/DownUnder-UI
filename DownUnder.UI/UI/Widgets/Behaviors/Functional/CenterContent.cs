using System;
using DownUnder.UI.Utilities.Extensions;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.Behaviors.Functional
{
    public class CenterContent : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        protected override void Initialize()
        {
            SetScroll(this, EventArgs.Empty);
            Parent.Behaviors.GroupBehaviors.AcceptancePolicy += GroupBehaviorAcceptancePolicy.NoUserScrolling;
        }

        protected override void ConnectEvents()
        {
            Parent.OnAreaChange += SetScroll;
            Parent.OnAddChild += SetScroll;
            Parent.OnRemoveChild += SetScroll;
            Parent.OnResize += SetScroll;
            Parent.OnChildAreaChange += SetScroll;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnAreaChange -= SetScroll;
            Parent.OnAddChild -= SetScroll;
            Parent.OnRemoveChild -= SetScroll;
            Parent.OnResize -= SetScroll;
            Parent.OnChildAreaChange += SetScroll;
        }

        public override object Clone()
        {
            return new CenterContent();
        }

        private void SetScroll(object sender, EventArgs args)
        {
            Parent.Scroll = NeededScroll;
        }

        private Point2 NeededScroll => Parent.Children.Count == 0 ? new Point2() : (Point2)(Parent.Size.DividedBy(2) - Parent.Children.AreaCoverage.Value.SizeOnly().Center);
    }
}
