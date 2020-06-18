using Downunder.Utility;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utility;
using MonoGame.Extended;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors
{
    [DataContract]
    public class BorderFormat : WidgetBehavior
    {
        private const string _POSITION_KEY = "P";
        private const string _CENTER_VALUE = "C";
        private const string _TOP_VALUE = "T";
        private const string _BOTTOM_VALUE = "B";
        private const string _LEFT_VALUE = "L";
        private const string _RIGHT_VALUE = "R";

        private readonly WidgetTracker _center;
        private readonly WidgetTracker _top_border;
        private readonly WidgetTracker _bottom_border;
        private readonly WidgetTracker _left_border;
        private readonly WidgetTracker _right_border;

        private bool _disable_align = false;

        public Widget Center
        {
            get => _center.Widget;
            set
            {
                _center.Widget = value;
                _Align();
            }
        }

        public Widget TopBorder
        {
            get => _top_border.Widget;
            set
            {
                _top_border.Widget = value;
                _Align();
            }
        }

        public Widget BottomBorder
        {
            get => _bottom_border.Widget;
            set
            {
                _bottom_border.Widget = value;
                _Align();
            }
        }

        public Widget LeftBorder
        {
            get => _left_border.Widget;
            set
            {
                _left_border.Widget = value;
                _Align();
            }
        }

        public Widget RightBorder
        {
            get => _right_border.Widget;
            set
            {
                _right_border.Widget = value;
                _Align();
            }
        }

        [DataMember] public GenericDirections2D<ChangingValue<float>> BorderOccupy { get; private set; } = new GenericDirections2D<ChangingValue<float>>(new ChangingValue<float>(1f));
        [DataMember] public BorderSize Spacing { get; set; } = new BorderSize(5f);

        public BorderFormat()
        {
            _center = new WidgetTracker(this, _POSITION_KEY, _CENTER_VALUE);
            _top_border = new WidgetTracker(this, _POSITION_KEY, _TOP_VALUE);
            _bottom_border = new WidgetTracker(this, _POSITION_KEY, _BOTTOM_VALUE);
            _left_border = new WidgetTracker(this, _POSITION_KEY, _LEFT_VALUE);
            _right_border = new WidgetTracker(this, _POSITION_KEY, _RIGHT_VALUE);

            _top_border.AddPersistentEvent(nameof(Widget.OnResize), Align);
            _bottom_border.AddPersistentEvent(nameof(Widget.OnResize), Align);
            _left_border.AddPersistentEvent(nameof(Widget.OnResize), Align);
            _right_border.AddPersistentEvent(nameof(Widget.OnResize), Align);
        }

        public override object Clone()
        {
            BorderFormat c = new BorderFormat();
            c.BorderOccupy = (GenericDirections2D<ChangingValue<float>>)BorderOccupy.Clone();
            c.Spacing = Spacing;
            return c;
        }

        protected override void ConnectToParent()
        {
            _center.FindIn(Parent.Children);
            _top_border.FindIn(Parent.Children);
            _bottom_border.FindIn(Parent.Children);
            _left_border.FindIn(Parent.Children);
            _right_border.FindIn(Parent.Children);
            Parent.OnResize += Align;
            Parent.OnUpdate += Update;
            _Align();
        }

        protected override void DisconnectFromParent()
        {
            _center.Forget();
            _top_border.Forget();
            _bottom_border.Forget();
            _left_border.Forget();
            _right_border.Forget();
            Parent.OnResize -= Align;
            Parent.OnUpdate -= Update;
        }

        public void Update(object sender, EventArgs args)
        {
            BorderOccupy.Up.Update(Parent.UpdateData.ElapsedSeconds);
            BorderOccupy.Down.Update(Parent.UpdateData.ElapsedSeconds);
            BorderOccupy.Left.Update(Parent.UpdateData.ElapsedSeconds);
            BorderOccupy.Right.Update(Parent.UpdateData.ElapsedSeconds);
        }

        private void Align(object sender, WidgetResizeEventArgs args)
        {
            _Align();
        }

        private void _Align()
        {
            if (_disable_align) return;
            _disable_align = true;

            BorderSize center_border = new BorderSize();

            if (TopBorder != null)
            {
                center_border.Top = TopBorder.Height * BorderOccupy.Up.GetCurrent();
                TopBorder.Position = new Point2();
                TopBorder.Width = Parent.Width;
            }

            if (BottomBorder != null)
            {
                center_border.Bottom = BottomBorder.Height * BorderOccupy.Down.GetCurrent();
                BottomBorder.Position = new Point2(0, Parent.Height - BottomBorder.Height);
                BottomBorder.Width = Parent.Width;
            }

            if (LeftBorder != null)
            {
                center_border.Left = LeftBorder.Width * BorderOccupy.Left.GetCurrent();
                LeftBorder.Position = new Point2(0, center_border.Top);
                LeftBorder.Height = Parent.Height - center_border.Top - center_border.Bottom;
            }

            if (RightBorder != null)
            {
                center_border.Right = RightBorder.Width * BorderOccupy.Right.GetCurrent();
                RightBorder.Position = new Point2(Parent.Width - RightBorder.Width, center_border.Top);
                RightBorder.Height = Parent.Height - center_border.Top - center_border.Bottom;
            }

            if (Center != null) Center.Area = Parent.Area.SizeOnly().ResizedBy(-center_border);
            _disable_align = false;
        }
    }
}
