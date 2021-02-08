using System;
using Microsoft.Xna.Framework;
using DownUnder.UI.UI.Widgets.Behaviors.Examples.Draw3DCubeActions;

namespace DownUnder.UI.UI.Widgets.Behaviors.Examples.Draw3DCubeBehaviors {
    public sealed class SpinOnHoverOnOff : WidgetBehavior, ISubWidgetBehavior<Draw3DCube> {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.FUNCTION };

        public Type BaseWidgetBehavior { get; } = typeof(Draw3DCube);

        public Draw3DCube BaseBehavior => Parent.Behaviors.Get<Draw3DCube>();

        public SpinCube Spin = new SpinCube {
            DuplicatePolicy = Actions.WidgetAction.DuplicatePolicyType.parallel,
            Direction = new Vector3(0.1f, 0f,0f)
        };

        protected override void Initialize() {
        }

        protected override void ConnectEvents() {
            Parent.OnHover += ApplySpin;
            Parent.OnHoverOff += ApplySpin;
        }

        protected override void DisconnectEvents() {
            Parent.OnHover += ApplySpin;
            Parent.OnHoverOff += ApplySpin;
        }

        public override object Clone() =>
            new SpinOnHoverOnOff {
                Spin = (SpinCube)Spin.InitialClone()
            };

        void ApplySpin(object sender, EventArgs args) =>
            Parent.Actions.Add((SpinCube)Spin.InitialClone());
    }
}
