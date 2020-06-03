using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors.Behaviors
{
    class SpacedListFormat : WidgetBehavior
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        protected override void ConnectToParent()
        {
            Parent.OnListChange += Align;
            Parent.OnAreaChange += Align;
            Align(this, EventArgs.Empty);
        }

        protected override void DisconnectFromParent()
        {
            Parent.OnListChange -= Align;
            Parent.OnAreaChange -= Align;
        }

        private void Align(object sender, EventArgs args)
        {

        }
    }
}
