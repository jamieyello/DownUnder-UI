using DownUnder.UI.Widgets.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors.Functional
{
    /// <summary> Used to "pin" the widget's position/size to some position within the parent widget. </summary>
    public class PinWidget : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        public InnerWidgetLocation Pin { get; set; }

        protected override void Initialize()
        {
        }

        protected override void ConnectEvents()
        {
            Parent.OnParentWidgetSet += ApplyPin;
            Parent.OnParentResize += ApplyPin;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnParentWidgetSet -= ApplyPin;
            Parent.OnParentResize -= ApplyPin;
        }

        public override object Clone()
        {
            PinWidget c = new PinWidget();
            c.Pin = (InnerWidgetLocation)Pin.Clone();
            return c;
        }

        private void ApplyPin(object sender, EventArgs args)
        {
            Pin.ApplyLocation(Parent.ParentWidget, Parent);
        }
    }
}
