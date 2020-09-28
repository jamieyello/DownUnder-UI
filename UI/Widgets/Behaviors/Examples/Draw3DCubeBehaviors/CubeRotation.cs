using Microsoft.Xna.Framework;
using System;

namespace DownUnder.UI.Widgets.Behaviors.Examples.Draw3DCubeBehaviors
{
    public class CubeRotation : WidgetBehavior, ISubWidgetBehavior<Draw3DCube>
    {
        public Vector3 Rotation = new Vector3();

        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        public Draw3DCube BaseBehavior => Parent.Behaviors.GetFirst<Draw3DCube>();

        public override object Clone()
        {
            CubeRotation c = new CubeRotation();
            c.Rotation = Rotation;
            return c;
        }

        protected override void ConnectEvents()
        {
            Parent.OnUpdate += Update;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnUpdate -= Update;
        }

        protected override void Initialize()
        {
            
        }

        private void Update(object sender, EventArgs args)
        {
            BaseBehavior.Angle += Rotation;
        }
    }
}
