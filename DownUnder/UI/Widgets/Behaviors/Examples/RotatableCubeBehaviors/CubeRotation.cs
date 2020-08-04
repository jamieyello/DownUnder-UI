using Microsoft.Xna.Framework;
using System;

namespace DownUnder.UI.Widgets.Behaviors.Examples.RotatableCubeBehaviors
{
    public class CubeRotation : WidgetBehavior, ISubWidgetBehavior
    {
        public Vector3 Rotation = new Vector3();

        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        public Type BaseWidgetBehavior { get; private set; } = typeof(RotatableCube);

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
            RotatableCube cube = Parent.Behaviors.GetFirst<RotatableCube>();
            if (cube == null) return;
            cube.Angle += Rotation;
        }
    }
}
