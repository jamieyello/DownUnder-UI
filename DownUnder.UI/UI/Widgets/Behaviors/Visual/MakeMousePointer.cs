using System;
using Microsoft.Xna.Framework.Input;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual {
    public sealed class MakeMousePointer : WidgetBehavior {
        public override string[] BehaviorIDs { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        public override object Clone() =>
            new MakeMousePointer();

        protected override void ConnectEvents() =>
            Parent.OnUpdate += Update;

        protected override void DisconnectEvents() =>
            Parent.OnUpdate -= Update;

        protected override void Initialize() {
        }

        void Update(object sender, EventArgs args) {
            if (!Parent.IsPrimaryHovered)
                return;

            Parent.ParentDWindow.UICursor = MouseCursor.Hand;
        }
    }
}
