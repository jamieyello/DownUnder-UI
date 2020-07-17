using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Actions.Functional
{
    class ActionSequence : WidgetAction
    {
        private int _current_action = 0;
        private readonly List<WidgetAction> _actions = new List<WidgetAction>();

        public override object InitialClone()
        {
            throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            throw new NotImplementedException();
        }

        protected override void ConnectToParent()
        {
            throw new NotImplementedException();
        }

        protected override void DisconnectFromParent()
        {
            throw new NotImplementedException();
        }

        protected override bool InterferesWith(WidgetAction action)
        {
            throw new NotImplementedException();
        }

        protected override bool Matches(WidgetAction action)
        {
            throw new NotImplementedException();
        }

        private void Update(object sender, EventArgs args)
        {

        }
    }
}
