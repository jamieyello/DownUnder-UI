using System;
using DownUnder.UI.UI.Widgets.Behaviors.Examples.Draw3DCubeActions;
using Microsoft.Xna.Framework;

namespace DownUnder.UI.UI.Widgets.Behaviors.Examples.Draw3DCubeBehaviors
{
    public class SpinOnHoverOnOff : WidgetBehavior, ISubWidgetBehavior<Draw3DCube>
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        public Type BaseWidgetBehavior { get; private set; } = typeof(Draw3DCube);

        public Draw3DCube BaseBehavior => Parent.Behaviors.Get<Draw3DCube>();

        public SpinCube Spin = new SpinCube()
        {
            DuplicatePolicy = Actions.WidgetAction.DuplicatePolicyType.parallel,
            Direction = new Vector3(0.1f, 0f,0f)
        };

        protected override void Initialize()
        {

        }

        protected override void ConnectEvents()
        {
            Parent.OnHover += ApplySpin;
            Parent.OnHoverOff += ApplySpin;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnHover += ApplySpin;
            Parent.OnHoverOff += ApplySpin;
        }

        public override object Clone()
        {
            SpinOnHoverOnOff c = new SpinOnHoverOnOff();
            c.Spin = (SpinCube)Spin.InitialClone();
            return c;
        }

        private void ApplySpin(object sender, EventArgs args)
        {
            Parent.Actions.Add((SpinCube)Spin.InitialClone());
        }
    }
}
