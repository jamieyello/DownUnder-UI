using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual.DrawTextBehaviors {
    [DataContract]
    public sealed class RepresentMember : WidgetBehavior, ISubWidgetBehavior<DrawText> {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.FUNCTION };

        public DrawText BaseBehavior => Parent.Behaviors.Get<DrawText>();
        internal bool UpdateTextInternal { get; set; } = true;

        MemberInfo _member;

        [DataMember] readonly string _name_of_member;
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
            Parent.OnUpdate += Update;

        protected override void DisconnectEvents() =>
            Parent.OnUpdate -= Update;

        public override object Clone()
        {
            var c = new RepresentMember(_represented_object, _name_of_member);
            c.UpdateTextInternal = UpdateTextInternal;
            return c;
        }


        void Update(object sender, EventArgs args)
        {
            if (UpdateTextInternal) UpdateText();
        }

        void UpdateText() {
            if (_member.GetParameters().Length != 0) {
                BaseBehavior.Text = "Collection...";
                return;
            }

            BaseBehavior.Text = _member.GetValue(_represented_object)?.ToString() ?? "null";
        }

        [OnDeserialized]
        void ThrowDeserializeError(StreamingContext context) =>
            throw new NotImplementedException();

        [OnSerialized]
        void ThrowSerializeError(StreamingContext context) =>
            throw new NotImplementedException();
    }
}