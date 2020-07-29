using DownUnder.UI.Widgets.Behaviors.Examples.RotatableCubeActions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors.Examples.RotatableCubeBehaviors
{
    public class SpinOnHoverOnOff : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

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
