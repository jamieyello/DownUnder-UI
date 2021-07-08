using System;
using DownUnder.UI.Widgets.DataTypes;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.Actions.Functional
{
    public sealed class ReplaceWidget : WidgetAction {
        DiagonalDirections2D _new_widget_snapping_policy_prev;
        PropertyTransitionAction<RectangleF> _new_widget_area;
        PropertyTransitionAction<RectangleF> _old_widget_area;

        public Widget NewWidget { get; set; }
        public InnerWidgetLocation NewWidgetStart { get; set; }
        public InnerWidgetLocation OldWidgetEnd { get; set; }
        public InterpolationSettings NewWidgetMovement { get; set; }
        public InterpolationSettings OldWidgetMovement { get; set; }

        bool DisposeOld;

        public ReplaceWidget(
            Widget new_widget,
            InnerWidgetLocation new_widget_start,
            InnerWidgetLocation old_widget_end,
            InterpolationSettings? new_widget_movement = null,
            InterpolationSettings? old_widget_movement = null,
            bool dispose_old = true
        ) {
            NewWidget = new_widget;
            NewWidgetStart = new_widget_start;
            OldWidgetEnd = old_widget_end;
            NewWidgetMovement = new_widget_movement ?? InterpolationSettings.Default;
            OldWidgetMovement = old_widget_movement ?? InterpolationSettings.Default;
            DisposeOld = dispose_old;
        }

        public ReplaceWidget(
            Widget new_widget,
            WidgetTransitionAnimation animation,
            bool dispose_old = true
        ) : this(
            new_widget,
            animation.NewWidgetStart,
            animation.OldWidgetEnd,
            animation.NewWidgetMovement,
            animation.OldWidgetMovement,
            dispose_old
        ) {
        }

        protected override void Initialize() {
            Parent.SendToContainer();
            Parent.ParentWidget.Insert(0, NewWidget);

            _new_widget_snapping_policy_prev = NewWidget.SnappingPolicy;
            NewWidget.SnappingPolicy = DiagonalDirections2D.None;

            var new_widget_target_area = NewWidget.Area;
            NewWidget.Area = NewWidgetStart.GetLocation(Parent, NewWidget);

            _old_widget_area = new PropertyTransitionAction<RectangleF>(nameof(Widget.Area), OldWidgetEnd.GetLocation(Parent, Parent), OldWidgetMovement);
            _new_widget_area = new PropertyTransitionAction<RectangleF>(nameof(Widget.Area), new_widget_target_area, NewWidgetMovement);

            Parent.Actions.Add(_old_widget_area);
            NewWidget.Actions.Add(_new_widget_area);
        }

        protected override void ConnectEvents() =>
            Parent.OnUpdate += Update;

        protected override void DisconnectEvents() =>
            Parent.OnUpdate -= Update;

        protected override bool InterferesWith(WidgetAction action) =>
            GetType() == action.GetType();

        protected override bool Matches(WidgetAction action) =>
            GetType() == action.GetType();

        public override object InitialClone() {
            var c = (ReplaceWidget)base.InitialClone();
            c.NewWidget = (Widget)NewWidget?.Clone();
            c.NewWidgetStart = (InnerWidgetLocation)NewWidgetStart?.Clone();
            c.OldWidgetEnd = (InnerWidgetLocation)OldWidgetEnd?.Clone();
            c.NewWidgetMovement = NewWidgetMovement;
            c.OldWidgetMovement = OldWidgetMovement;
            c.DisposeOld = DisposeOld;
            return c;
        }

        void Update(object sender, EventArgs args) {
            if (_old_widget_area.IsCompleted && _new_widget_area.IsCompleted)
                Complete();
        }

        void Complete() {
            NewWidget.SnappingPolicy = _new_widget_snapping_policy_prev;
            if (!DisposeOld && !Parent.ParentWidget.Remove(Parent))
                throw new Exception();
            NewWidget.ReplaceContainer();
            EndAction();
        }
    }
}