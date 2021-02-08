using System;
using System.Collections.Generic;

namespace DownUnder.UI.UI.Widgets.Actions.Functional {
    sealed class ActionSequence : WidgetAction {
        int _current_action = 0;
        readonly List<WidgetAction> _actions = new List<WidgetAction>();

        public override object InitialClone() => throw new NotImplementedException();
        protected override void Initialize() => throw new NotImplementedException();
        protected override void ConnectEvents() => throw new NotImplementedException();
        protected override void DisconnectEvents() => throw new NotImplementedException();
        protected override bool InterferesWith(WidgetAction action) => throw new NotImplementedException();
        protected override bool Matches(WidgetAction action) => throw new NotImplementedException();

        void Update(object sender, EventArgs args) {
        }
    }
}
