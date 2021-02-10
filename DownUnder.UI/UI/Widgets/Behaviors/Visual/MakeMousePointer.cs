using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Input;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual {
    [DataContract] 
    public sealed class MakeMousePointer : WidgetBehavior {
        public override string[] BehaviorIDs { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        [DataMember] public bool Enabled { get; set; } = true;
        /// <summary> If true this will disable if <see cref="Widget.IsActive"/> is false. true by default. </summary>
        [DataMember] public bool RespectActivation { get; set; } = true;

        public override object Clone() {
            var c = new MakeMousePointer();
            c.Enabled = Enabled;
            c.RespectActivation = RespectActivation;
            return c;
        }

        protected override void ConnectEvents() =>
            Parent.OnUpdate += Update;

        protected override void DisconnectEvents() =>
            Parent.OnUpdate -= Update;

        protected override void Initialize() {
        }

        void Update(object sender, EventArgs args) {
            if (!Parent.IsPrimaryHovered || (RespectActivation && !Parent.IsActive))
                return;

            Parent.ParentDWindow.UICursor = MouseCursor.Hand;
        }
    }
}
