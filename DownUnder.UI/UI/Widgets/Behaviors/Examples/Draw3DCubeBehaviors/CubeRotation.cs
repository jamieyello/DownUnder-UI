using System;
using Microsoft.Xna.Framework;

namespace DownUnder.UI.UI.Widgets.Behaviors.Examples.Draw3DCubeBehaviors {
    public sealed class CubeRotation : WidgetBehavior, ISubWidgetBehavior<Draw3DCube> {
        public Vector3 Rotation;

        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.FUNCTION };

        public Draw3DCube BaseBehavior => Parent.Behaviors.Get<Draw3DCube>();

        public override object Clone() =>
            new CubeRotation {
                Rotation = Rotation
            };

        protected override void ConnectEvents() => Parent.OnUpdate += Update;
        protected override void DisconnectEvents() => Parent.OnUpdate -= Update;
        protected override void Initialize() { }

        void Update(object sender, EventArgs args) =>
            BaseBehavior.Angle += Rotation;
    }
}
