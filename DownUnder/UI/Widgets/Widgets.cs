using DownUnder.UI.Widgets.BaseWidgets;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets
{
    public static class WidgetStuff {
        public static BorderedContainer MenuBar(Widget parent = null, Widget widget = null) {
            BorderedContainer container = new BorderedContainer();
            container.Size = new Point2(400f, 300f);
            SpacedList menu = new SpacedList(container);
            menu.Height = 50f;
            menu.Add(new Label(null, "stuff"));
            container.Borders.Up.Widget = menu;

            return container;
        }
    }
}
