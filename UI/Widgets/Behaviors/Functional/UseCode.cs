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
            Code.ConnectMatches(Parent);
        }

        protected override void DisconnectEvents()
        {
            throw new NotImplementedException();
        }

        public override object Clone()
        {
            return new UseCode(Code);
        }
    }
}
