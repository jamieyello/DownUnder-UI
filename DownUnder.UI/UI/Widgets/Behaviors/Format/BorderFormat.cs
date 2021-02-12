using System;
using System.Runtime.Serialization;
using DownUnder.UI.UI.Widgets.CustomEventArgs;
using DownUnder.UI.UI.Widgets.DataTypes;
using DownUnder.UI.Utilities.CommonNamespace;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.Behaviors.Format {
    [DataContract]
    public sealed class BorderFormat : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.FUNCTION };

        const string _POSITION_KEY = "P";
        const string _CENTER_VALUE = "C";
        const string _TOP_VALUE = "T";
        const string _BOTTOM_VALUE = "B";
        const string _LEFT_VALUE = "L";
        const string _RIGHT_VALUE = "R";

        readonly WidgetTracker _center;
        readonly WidgetTracker _top_border;
        readonly WidgetTracker _bottom_border;
        readonly WidgetTracker _left_border;
        readonly WidgetTracker _right_border;

        bool _disable_align;

        public Widget Center {
            get => _center.Widget;
            set {
                if (value is { })
                    value.SnappingPolicy = DiagonalDirections2D.None;

                _center.Widget = value;
                _Align();
                value?.SendToBack();
            }
        }

        public Widget TopBorder {
            get => _top_border.Widget;
            set {
                if (value is { })
                    value.SnappingPolicy = DiagonalDirections2D.None;

                _top_border.Widget = value;
                _Align();
            }
        }

        public Widget BottomBorder {
            get => _bottom_border.Widget;
            set {
                if (value is { })
                    value.SnappingPolicy = DiagonalDirections2D.None;

                _bottom_border.Widget = value;
                _Align();
            }
        }

        public Widget LeftBorder {
            get => _left_border.Widget;
            set {
                if (value is { })
                    value.SnappingPolicy = DiagonalDirections2D.None;

                _left_border.Widget = value;
                _Align();
            }
        }

        public Widget RightBorder {
            get => _right_border.Widget;
            set {
                if (value is { })
                    value.SnappingPolicy = DiagonalDirections2D.None;

                _right_border.Widget = value;
                _Align();
            }
        }

        [DataMember] public GenericDirections2D<ChangingValue<float>> BorderOccupy { get; private set; } = new GenericDirections2D<ChangingValue<float>>(new ChangingValue<float>(1f));
        [DataMember] public BorderSize Spacing { get; set; } = new BorderSize(0f);

        public BorderFormat() {
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

        ~BorderFormat() {
            _center?.Forget();
            _top_border?.Forget();
            _bottom_border?.Forget();
            _left_border?.Forget();
            _right_border?.Forget();
        }

        public override object Clone() =>
            new BorderFormat {
                BorderOccupy = (GenericDirections2D<ChangingValue<float>>)BorderOccupy.Clone(),
                Spacing = Spacing
            };

        protected override void Initialize() {
            Parent.VisualSettings.DrawBackground = false;
            Parent.Behaviors.GroupBehaviors.AcceptancePolicy += GroupBehaviorAcceptancePolicy.NoUserScrolling;
            _center.FindIn(Parent.Children);
            _top_border.FindIn(Parent.Children);
            _bottom_border.FindIn(Parent.Children);
            _left_border.FindIn(Parent.Children);
            _right_border.FindIn(Parent.Children);
            _Align();
        }

        protected override void ConnectEvents() {
            Parent.OnResize += Align;
            Parent.OnUpdate += Update;
        }

        protected override void DisconnectEvents() {
            Parent.OnResize -= Align;
            Parent.OnUpdate -= Update;
        }

        public void Update(object sender, EventArgs args) {
            var align =
                BorderOccupy.Up.IsTransitioning
                || BorderOccupy.Down.IsTransitioning
                || BorderOccupy.Left.IsTransitioning
                || BorderOccupy.Right.IsTransitioning;

            BorderOccupy.Up.Update(Parent.UpdateData.ElapsedSeconds);
            BorderOccupy.Down.Update(Parent.UpdateData.ElapsedSeconds);
            BorderOccupy.Left.Update(Parent.UpdateData.ElapsedSeconds);
            BorderOccupy.Right.Update(Parent.UpdateData.ElapsedSeconds);

            if (align) _Align();
        }

        void Align(object sender, RectangleFSetArgs args) =>
            _Align();

        void _Align() {
            if (_disable_align)
                return;

            _disable_align = true;

            var center_border = new BorderSize();

            if (TopBorder != null) {
                center_border = center_border.SetTop(TopBorder.Height * BorderOccupy.Up.Current);
                TopBorder.Position = new Point2();
                TopBorder.Width = Parent.Width;
            }

            if (BottomBorder != null) {
                center_border = center_border.SetBottom(BottomBorder.Height * BorderOccupy.Down.Current);
                BottomBorder.Position = new Point2(0, Parent.Height - BottomBorder.Height);
                BottomBorder.Width = Parent.Width;
            }

            if (LeftBorder != null) {
                center_border = center_border.SetLeft(LeftBorder.Width * BorderOccupy.Left.Current);
                LeftBorder.Position = new Point2(0, center_border.Top);
                LeftBorder.Height = Parent.Height - center_border.Top - center_border.Bottom;
            }

            if (RightBorder != null) {
                center_border = center_border.SetRight(RightBorder.Width * BorderOccupy.Right.Current);
                RightBorder.Position = new Point2(Parent.Width - RightBorder.Width, center_border.Top);
                RightBorder.Height = Parent.Height - center_border.Top - center_border.Bottom;
            }

            if (Center != null)
                Center.Area = Parent.Area.SizeOnly().ResizedBy(-center_border);

            _disable_align = false;
        }
    }
}