using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors;
using DownUnder.Utility;
using MonoGame.Extended;

namespace DownUnder.UIEditor
{
    internal static class TestLayouts
    {
        public static Widget NewLayout()
        {
            Widget layout = new Widget();

            Widget new_widget = new Widget()
            {
                SnappingPolicy = DiagonalDirections2D.None,
                Area = new RectangleF(100, 100, 200, 150)
            };
            layout.Add(new_widget);

            GridFormat grid_formatter = new GridFormat(2, 2);
            new_widget.Behaviors.Add(grid_formatter);
            grid_formatter[0, 0].Behaviors.Add(new DrawText() { Text = "test" });

            return layout;
        }

        public static Widget ContainerTest()
        {
            Widget layout = new Widget();

            Widget inner = new Widget()
            {
                SnappingPolicy = DiagonalDirections2D.None,
                Area = new RectangleF(40, 40, 400, 300)
            };

            inner.Behaviors.Add(new BorderFormat(), out var border_format);
            border_format.TopBorder = new Widget();
            border_format.Center = new Widget();
            border_format.BottomBorder = new Widget();
            border_format.LeftBorder = new Widget();
            border_format.RightBorder = new Widget();
            layout.Add(inner);

            inner.UserResizePolicy = Widget.UserResizePolicyType.allow;
            inner.AllowedResizingDirections = Directions2D.All;

            return layout;
        }
    }
}