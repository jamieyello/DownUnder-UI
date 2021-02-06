using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DownUnder.UI.Widgets.Actions.Functional
{
    [DataContract]
    public class DirectAction : WidgetAction
    {
        //[DataMember]
        public Action Action;

        public DirectAction() { }
        public DirectAction(Action action)
        {
            Action = action;
        }

        protected override void Initialize()
        {
            Action.Invoke();
            EndAction();
        }

        protected override void ConnectEvents()
        {

        }

        protected override void DisconnectEvents()
        {

        }

        protected override bool InterferesWith(WidgetAction action)
        {
            return action.GetType() == GetType();
        }

        protected override bool Matches(WidgetAction action)
        {
            return action.GetType() == GetType();
        }

        public override object InitialClone()
        {
            DirectAction result = (DirectAction)base.InitialClone();
            result.Action = Action;
            return result;
        }
    }
}
