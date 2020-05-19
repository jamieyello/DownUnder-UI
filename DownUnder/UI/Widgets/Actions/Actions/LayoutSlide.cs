using DownUnder.UI.Widgets.BaseWidgets;
using DownUnder.Utilities;
using DownUnder.Utility;
using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets.Actions.Actions
{
    class AutoLayoutSlide : WidgetAction
    {
        private readonly Widget _new_widget;
        private readonly Directions2D _direction;
        private readonly InterpolationSettings _interpolation;

        private Point2 _new_widget_start;
        private ChangingValue<float> _progress;

        public AutoLayoutSlide(Widget new_widget, Directions2D direction, InterpolationSettings? interpolation = null)
        {
            if (_direction.HasMultiple) throw new Exception($"Cannot use multiple directions in {nameof(AutoLayoutSlide)}.");
            if (_direction == Directions2D.None) throw new Exception($"No direction given.");
            _new_widget = new_widget;
            _direction = direction;
            _interpolation = interpolation == null ? InterpolationSettings.Default : interpolation.Value;
        }

        public override object InitialClone()
        {
            throw new NotImplementedException();
        }

        protected override void ConnectToParent()
        {
            if (!(Parent is Layout)) throw new Exception($"{nameof(AutoLayoutSlide)} can only be used with {nameof(Layout)}.");
            _new_widget.Parent = Parent;
            Parent.OnUpdate += Update;
            _progress = new ChangingValue<float>(0f, _interpolation);
            _new_widget_start = _direction.ValueInDirection(Parent.Size);
            Align();
        }

        internal override void DisconnectFromParent()
        {
            Parent.OnUpdate -= Update;
        }

        internal override bool Matches(WidgetAction action) => action is AutoLayoutSlide;

        private void Update(object sender, EventArgs args)
        {
            _progress.Update(Parent.UpdateData.ElapsedSeconds);
            Align();
        }

        private void Align()
        {
            _new_widget.Position = Interpolation.GetMiddle(_new_widget_start, new Point2(), _progress.GetCurrent(), InterpolationType.linear);
        }
    }
}
