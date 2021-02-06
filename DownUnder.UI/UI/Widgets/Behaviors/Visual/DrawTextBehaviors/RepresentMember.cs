using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors.Visual.DrawTextBehaviors
{
    public class RepresentMember : WidgetBehavior, ISubWidgetBehavior<DrawText>
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        public DrawText BaseBehavior => Parent.Behaviors.Get<DrawText>();

        private MemberInfo _member;

        public string NameOfMember;
        public object RepresentedObject;

        public RepresentMember() { }
        public RepresentMember(object obj, string nameof_member)
        {
            RepresentedObject = obj;
            NameOfMember = nameof_member;
        }

        protected override void Initialize()
        {
            _member = RepresentedObject.GetType().GetMember(NameOfMember)[0];
            UpdateText();
        }

        protected override void ConnectEvents()
        {
            Parent.OnUpdate += UpdateText;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnUpdate -= UpdateText;
        }

        public override object Clone()
        {
            return new RepresentMember(RepresentedObject, NameOfMember);
        }

        private void UpdateText(object sender, EventArgs args)
        {
            if (_member.GetParameters().Length != 0)
            {
                BaseBehavior.Text = "Collection...";
                return;
            }

            BaseBehavior.Text = _member.GetValue(RepresentedObject)?.ToString() ?? "null";
        }

        public void UpdateText()
        {
            UpdateText(this, EventArgs.Empty);
        }
    }
}
