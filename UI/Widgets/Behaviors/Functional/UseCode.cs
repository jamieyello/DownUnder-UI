using DownUnder.UI.Widgets.WidgetCoding;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Functional
{
    [DataContract]
    public class UseCode : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        [DataMember] public WidgetCode Code { get; private set; }

        public UseCode()
        {

        }

        public UseCode(WidgetCode code)
        {
            Code = code;
        }

        protected override void Initialize() { }

        protected override void ConnectEvents()
        {
            Code.Connect(Parent);
        }

        protected override void DisconnectEvents()
        {
            Code.Disconnect(Parent);
        }

        public override object Clone()
        {
            return new UseCode(Code);
        }
    }
}
