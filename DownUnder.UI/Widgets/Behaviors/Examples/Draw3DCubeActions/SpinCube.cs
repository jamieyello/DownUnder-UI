using System;
using Microsoft.Xna.Framework;
using DownUnder.UI.Widgets.Actions;
using DownUnder.UI.Utilities;

namespace DownUnder.UI.Widgets.Behaviors.Examples.Draw3DCubeActions
{
    public sealed class SpinCube : WidgetAction {
        public Vector3 Direction;
        InterpolationSettings _interpolation;
        ChangingValue<Vector3> _inertia;

        public SpinCube() =>
            _interpolation = new InterpolationSettings {
                Interpolation = InterpolationType.fake_sin,
                TransitionSpeed = 0.6f
            };

        protected override void ConnectEvents() =>
            Parent.OnUpdate += Update;

        protected override void DisconnectEvents() =>
            Parent.OnUpdate -= Update;

        protected override void Initialize() =>
            _inertia = new ChangingValue<Vector3>(Direction, new Vector3(), _interpolation);

        protected override bool InterferesWith(WidgetAction action) =>
            false;

        protected override bool Matches(WidgetAction action) =>
            action is SpinCube;

        public override object InitialClone() {
            var c = (SpinCube)base.InitialClone();
            c.Direction = Direction;
            c._interpolation = _interpolation;
            return c;
        }

        void Update(object sender, EventArgs args) {
            var cube = Parent.Behaviors.Get<Draw3DCube>();
            if (cube == null)
                return;

            cube.Angle += _inertia.Current;
            _inertia.Update(Parent.UpdateData.ElapsedSeconds);

            if (!_inertia.IsTransitioning)
                EndAction();
        }
    }
}
