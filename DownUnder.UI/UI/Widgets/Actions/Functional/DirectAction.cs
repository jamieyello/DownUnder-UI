using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.UI.Widgets.Actions.Functional {
    [DataContract]
    public sealed class DirectAction : WidgetAction {
        //[DataMember]
        public Action Action;

        public DirectAction() {
        }

        public DirectAction(Action action) =>
            Action = action;

        protected override void Initialize() {
            Action.Invoke();
            EndAction();
        }

        protected override void ConnectEvents() {
        }

        protected override void DisconnectEvents() {
        }

        protected override bool InterferesWith(WidgetAction action) =>
            action.GetType() == GetType();

        protected override bool Matches(WidgetAction action) =>
            action.GetType() == GetType();

        public override object InitialClone() {
            var result = (DirectAction)base.InitialClone();
            result.Action = Action;
            return result;
        }
    }
}
