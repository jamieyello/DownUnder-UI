using DownUnder.UI.Widgets.Actions.Functional;
using DownUnder.UI.Widgets.Behaviors.Format;
using DownUnder.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.Widgets.DataTypes;

namespace DownUnder.UI.Widgets
{
    public static class CommonWidgets
    {
        public static Widget WindowHandle(Widget handle)
        {
            Widget bar = new Widget();
            Widget x_button = BasicWidgets.ImageButton("DownUnder Native Content/Images/Window X", 0.5f);
            Widget __button = BasicWidgets.ImageButton("DownUnder Native Content/Images/Window _", 0.5f);
            Widget rect_button = BasicWidgets.ImageButton("DownUnder Native Content/Images/Window Rect", 0.5f);
            Widget control_grid = new Widget() { Size = new MonoGame.Extended.Point2(70,70) };

            control_grid.Add(__button);
            control_grid.Add(rect_button);
            control_grid.Add(x_button);

            control_grid.Behaviors.Add(new GridFormat(3, 1));
            control_grid.Size = new MonoGame.Extended.Point2(180, 40);

            __button.VisualSettings.DrawOutline = false;
            rect_button.VisualSettings.DrawOutline = false;
            x_button.VisualSettings.DrawOutline = false;
            control_grid.VisualSettings.DrawOutline = false;

            __button.VisualSettings.DrawBackground = false;
            rect_button.VisualSettings.DrawBackground = false;
            x_button.VisualSettings.DrawBackground = false;
            control_grid.VisualSettings.DrawBackground = false;

            //bar.VisualSettings.
            bar.Add(control_grid);
            var text = new DrawText(handle.Window.Title ?? "DownUnderCrossPlatform");
            text.SideSpacing = 8f;
            text.YTextPositioning = DrawText.YTextPositioningPolicy.center;
            bar.Behaviors.Add(text);

            bar.MinimumHeight = 40;

            x_button.OnClick += (s, a) => { handle.Delete(); };
            rect_button.Behaviors.Add(new TriggerAction(nameof(Widget.OnClick), new ToggleWindowFullscreen()));
            control_grid.Behaviors.Add(new PinWidget() { Pin = InnerWidgetLocation.InsideTopRight });
            bar.Behaviors.Add(new TriggerAction(nameof(Widget.OnDoubleClick), new ToggleWindowFullscreen()));

            return bar;
        }
    }
}
