using DownUnder.UI.Widgets.Actions;
using DownUnder.Utility;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors
{
    public class DropDownFormat : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        public bool DeleteOnClickOff { get; set; } = true;
        Directions2D StartSide { get; set; } = Directions2D.U;

        protected override void Initialize()
        {
            Parent.MinimumSize = new Point2(1f, 1f);
            Parent.GroupBehaviors.AcceptancePolicy += GroupBehaviorAcceptancePolicy.NonScrollable;
            RectangleF area = Parent.Area;
            Parent.Height = 0f;
            Parent.Actions.Add(new PropertyTransitionAction<RectangleF>(nameof(Widget.Area), area));
        }

        protected override void ConnectEvents()
        {
            Parent.OnClickOff += Close;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnClickOff -= Close;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        private void Close(object sender, EventArgs args)
        {

        }
    }
}
