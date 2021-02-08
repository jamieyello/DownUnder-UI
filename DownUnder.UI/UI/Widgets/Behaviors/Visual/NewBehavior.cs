using System;
using DownUnder.UI.UI.Widgets.CustomEventArgs;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual {
    sealed class NewBehavior : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.FUNCTION };

        /// <summary> Called when the parent widget is set. Use for initialization logic. </summary>
        protected override void Initialize() {
        }

        /// <summary> All Events should be added here. </summary>
        protected override void ConnectEvents() {
            Parent.OnUpdate += Update;
            Parent.OnDraw += Draw;
        }

        /// <summary> All Events added in ConnectEvents should be removed here. </summary>
        protected override void DisconnectEvents() {
            Parent.OnUpdate -= Update;
            Parent.OnDraw -= Draw;
        }

        /// <summary> Return a copy of this behavior. This is used internally by the framework. </summary>
        public override object Clone() =>
            new NewBehavior();

        void Update(object sender, EventArgs args) {
        }

        void Draw(object sender, WidgetDrawArgs args) {
        }
    }
}