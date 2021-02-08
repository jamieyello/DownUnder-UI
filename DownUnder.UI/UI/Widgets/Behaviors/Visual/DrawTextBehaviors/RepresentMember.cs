using System;
using System.Reflection;
using DownUnder.UI.Utilities.Extensions;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual.DrawTextBehaviors {
    public sealed class RepresentMember : WidgetBehavior, ISubWidgetBehavior<DrawText> {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.FUNCTION };

        public DrawText BaseBehavior => Parent.Behaviors.Get<DrawText>();

        MemberInfo _member;

        readonly string _name_of_member;
        readonly object _represented_object;

        public RepresentMember(object obj, string nameof_member) {
            _represented_object = obj;
            _name_of_member = nameof_member;
        }

        protected override void Initialize() {
            _member = _represented_object.GetType().GetMember(_name_of_member)[0];
            UpdateText();
        }

        protected override void ConnectEvents() =>
            Parent.OnUpdate += UpdateText;

        protected override void DisconnectEvents() =>
            Parent.OnUpdate -= UpdateText;

        public override object Clone() =>
            new RepresentMember(_represented_object, _name_of_member);

        void UpdateText(object sender, EventArgs args) {
            if (_member.GetParameters().Length != 0) {
                BaseBehavior.Text = "Collection...";
                return;
            }

            BaseBehavior.Text = _member.GetValue(_represented_object)?.ToString() ?? "null";
        }

        public void UpdateText() =>
            UpdateText(this, EventArgs.Empty);
    }
}