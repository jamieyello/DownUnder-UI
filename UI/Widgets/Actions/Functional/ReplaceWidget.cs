using DownUnder;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Actions;
using DownUnder.UI.Widgets.Actions.Functional;
using DownUnder.UI.Widgets.DataTypes;
using MonoGame.Extended;
using System;

public class ReplaceWidget : WidgetAction
{
    DiagonalDirections2D _new_widget_snapping_policy_prev;
    PropertyTransitionAction<RectangleF> _new_widget_area;
    PropertyTransitionAction<RectangleF> _old_widget_area;

    public Widget NewWidget { get; set; }
    public InnerWidgetLocation NewWidgetStart { get; set; }
    public InnerWidgetLocation OldWidgetEnd { get; set; }
    public InterpolationSettings NewWidgetMovement { get; set; }
    public InterpolationSettings OldWidgetMovement { get; set; }

    public ReplaceWidget(
        Widget new_widget, 
        InnerWidgetLocation new_widget_start,
        InnerWidgetLocation old_widget_end,
        InterpolationSettings? new_widget_movement = null, 
        InterpolationSettings? old_widget_movement = null)
    {
        NewWidget = new_widget;
        NewWidgetStart = new_widget_start;
        OldWidgetEnd = old_widget_end;
        NewWidgetMovement = new_widget_movement ?? InterpolationSettings.Default;
        OldWidgetMovement = old_widget_movement ?? InterpolationSettings.Default;
    }

    protected override void Initialize()
    {
        Parent.SendToContainer();
        Parent.ParentWidget.Insert(0, NewWidget);

        _new_widget_snapping_policy_prev = NewWidget.SnappingPolicy;
        NewWidget.SnappingPolicy = DiagonalDirections2D.None;

        RectangleF new_widget_target_area = NewWidget.Area;
        NewWidget.Area = NewWidgetStart.GetLocation(Parent, NewWidget);

        _old_widget_area = new PropertyTransitionAction<RectangleF>(nameof(Widget.Area), OldWidgetEnd.GetLocation(Parent, Parent), OldWidgetMovement);
        _new_widget_area = new PropertyTransitionAction<RectangleF>(nameof(Widget.Area), new_widget_target_area, NewWidgetMovement);

        Parent.Actions.Add(_old_widget_area);
        NewWidget.Actions.Add(_new_widget_area);
    }

    protected override void ConnectEvents()
    {
        Parent.OnUpdate += Update;
    }

    protected override void DisconnectEvents()
    {
        Parent.OnUpdate -= Update;
    }

    protected override bool InterferesWith(WidgetAction action)
    {
        return action.GetType() == action.GetType();
    }

    protected override bool Matches(WidgetAction action)
    {
        return action.GetType() == action.GetType();
    }

    public override object InitialClone()
    {
        ReplaceWidget c = (ReplaceWidget)base.InitialClone();
        c.NewWidget = (Widget)NewWidget?.Clone();
        c.NewWidgetStart = (InnerWidgetLocation)NewWidgetStart?.Clone();
        c.OldWidgetEnd = (InnerWidgetLocation)OldWidgetEnd?.Clone();
        c.NewWidgetMovement = NewWidgetMovement;
        c.OldWidgetMovement = OldWidgetMovement;
        return c;
    }

    void Update(object sender, EventArgs args)
    {
        if (_old_widget_area.IsCompleted && _new_widget_area.IsCompleted) Complete();
    }

    void Complete()
    {
        NewWidget.SnappingPolicy = _new_widget_snapping_policy_prev;
        NewWidget.ReplaceContainer();
        EndAction();
    }
}