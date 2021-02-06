using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets.DataTypes
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
        public static Widget Get(WidgetList widgets, int width, int height, int x, int y) {
            if (x >= width || y >= height) throw new Exception("Index out of bounds.");
            return widgets[y * width + x];
        }

        public static Point IndexOf(int width, int index) =>
            new Point(index % width, index / width);

        /// <summary> The total height/width of contained widgets that won't resize. </summary>
        public static Point2 FixedContentSizeTotal(WidgetList widgets, int width, int height) {
            Point2 fixed_size = new Point2();
            for (int i = 0; i < width; i++) {
                float fixed_width = FixedWidthOfColumn(widgets, width, height, i);
                if (fixed_width != -1f) fixed_size.X += fixed_width;
            }

            for (int i = 0; i < height; i++) {
                float fixed_height = FixedHeightOfRow(widgets, width, i);
                if (fixed_height != -1f) fixed_size.Y += fixed_height;
            }

            return fixed_size;
        }

        public static float FixedHeightOfRow(WidgetList widgets, int width, int row) {
            foreach (Widget widget in GetRow(widgets, width, row)) {
                if (widget.IsFixedHeight) return widget.Height;
            }

            return -1f;
        }

        public static float FixedWidthOfColumn(WidgetList widgets, int width, int height, int column) {
            foreach (Widget widget in GetColumn(widgets, width, height, column)) {
                if (widget.IsFixedWidth) return widget.Width;
            }

            return -1f;
        }
    }
}