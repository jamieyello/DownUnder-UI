using DownUnder.UI.Widgets.BaseWidgets;
using DownUnder.Utilities;
using DownUnder.Utility;
using MonoGame.Extended;
using System;
using System.Data;

namespace DownUnder.UI.Widgets.Actions
{
    public class AutoLayoutSlide : WidgetAction
    {
        private readonly Widget _new_widget;
        private readonly Directions2D _direction;
        private readonly InterpolationSettings _interpolation;

        private Widget _old_widget;
        private RectangleF _new_widget_start;
        private RectangleF _new_widget_end;
        private ChangingValue<RectangleF> _new_widget_area;

        public AutoLayoutSlide(Widget new_widget, Directions2D direction, InterpolationSettings? interpolation = null)
        {
            if (direction.HasMultiple) throw new Exception($"Cannot use multiple directions in {nameof(AutoLayoutSlide)}.");
            if (direction == Directions2D.None) throw new Exception($"No direction given.");
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
            if (!(Parent is Layout p_layout)) throw new Exception($"{nameof(AutoLayoutSlide)} can only be used with {nameof(Layout)}.");
            if (Parent.Children.Count != 1) throw new Exception($"Parent {nameof(Layout)} can only have one child for this to work.");
            _old_widget = p_layout.Children[0];
            p_layout.Add(_new_widget);
            Parent.OnUpdate += Update;
            _new_widget_start = _direction.ValueInDirection(Parent.Size).AsRectanglePosition(_new_widget.Size);
            _new_widget_end = _new_widget.Area;
            _new_widget_area = new ChangingValue<RectangleF>(_new_widget_start, _new_widget_end, _interpolation);
            
        }

        protected override void DisconnectFromParent()
        {
            Parent.OnUpdate -= Update;
            _old_widget.Delete();
        }

        protected override bool InterferesWith(WidgetAction action) => action is AsyncLayoutSlide || action is AutoLayoutSlide;

        private void Update(object sender, EventArgs args)
        {
            _new_widget_area.Update(Parent.UpdateData.ElapsedSeconds);
            Align();
            if (!_new_widget_area.IsTransitioning) EndAction();
        }

        private void Align()
        {
            _new_widget.Position = _new_widget_area.GetCurrent().Position;
            _old_widget.Position = _new_widget_start.Position.WithOffset(_new_widget_end.Position).MultipliedBy(_new_widget_area.ProgressPlotted).Inverted();
        }

        protected override bool Matches(WidgetAction action)
        {
            throw new NotImplementedException();
        }
    }
}
