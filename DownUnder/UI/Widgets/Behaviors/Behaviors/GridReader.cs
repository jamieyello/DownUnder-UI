using DownUnder.UI.Widgets.DataTypes;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.Behaviors
{
    /// <summary> Allows reading of <see cref="WidgetList"/> in a grid pattern. </summary>
    public static class GridReader
    {
        public static WidgetList GetRow(WidgetList widgets, int width, int y_row) =>
            widgets.GetRange(y_row * width, width);

        public static WidgetList GetColumn(WidgetList widgets, int width, int height, int x_column) {
            WidgetList result = new WidgetList();
            for (int y = 0; y < height; y++) result.Add(widgets[y * width + x_column]);
            return result;
        }

        /// <summary> Returns the reference of a Widget at a given coordinate. </summary>
        public static Widget Get(WidgetList widgets, int width, int x, int y) =>
            widgets[x % width + (y / width) * width];
        

        public static Point2 GridIndexOf(int width, int index) => 
            new Point2(index % width, (index / width) * width);
    }
}