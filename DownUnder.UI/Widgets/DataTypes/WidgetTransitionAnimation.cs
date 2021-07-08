namespace DownUnder.UI.Widgets.DataTypes
{
    public sealed class WidgetTransitionAnimation {
        public InnerWidgetLocation NewWidgetStart { get; }
        public InnerWidgetLocation OldWidgetEnd { get; }
        public InterpolationSettings NewWidgetMovement { get; }
        public InterpolationSettings OldWidgetMovement { get; }

        public WidgetTransitionAnimation(
            InnerWidgetLocation new_widget_start,
            InnerWidgetLocation old_widget_end,
            InterpolationSettings new_widget_movement,
            InterpolationSettings old_widget_movement
        ) {
            NewWidgetStart = new_widget_start;
            OldWidgetEnd = old_widget_end;
            NewWidgetMovement = new_widget_movement;
            OldWidgetMovement = old_widget_movement;
        }

        public static WidgetTransitionAnimation Slide(
            Direction2D direction,
            InterpolationSettings? interpolation = null
        ) =>
            new WidgetTransitionAnimation(
                InnerWidgetLocation.Outside(Directions2D.GetOppositeSide(direction)),
                InnerWidgetLocation.Outside(direction),
                interpolation ?? InterpolationSettings.Fast,
                interpolation ?? InterpolationSettings.Fast
            );
    }
}
