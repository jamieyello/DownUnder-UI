using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors.Visual.DrawTextBehaviors
{
    public class DisplayObjectMember : WidgetBehavior, ISubWidgetBehavior<DrawText>
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        public DrawText BaseBehavior => Parent.Behaviors.GetFirst<DrawText>();

        private MemberInfo _member;

        public string NameOfMember;
        public object RepresentedObject;

        public DisplayObjectMember() { }
        public DisplayObjectMember(object obj, string nameof_member)
        {
            RepresentedObject = obj;
            NameOfMember = nameof_member;
        }

        protected override void Initialize()
        {
            _member = RepresentedObject.GetType().GetMember(NameOfMember)[0];
            UpdateText();
        }

        protected override void ConnectEvents() { }

        protected override void DisconnectEvents() { }

        public override object Clone()
        {
            return new DisplayObjectMember(RepresentedObject, NameOfMember);
        }

        private void UpdateText(object sender, EventArgs args)
        {
            BaseBehavior.Text = _member.GetValue(RepresentedObject).ToString();
        }

        public void UpdateText()
        {
            UpdateText(this, EventArgs.Empty);
        }
    }
}
