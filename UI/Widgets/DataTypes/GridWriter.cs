using MonoGame.Extended;
using System.Collections.Generic;
using DownUnder;

namespace DownUnder.UI.Widgets.DataTypes
{
    /// <summary> Allows aligning a <see cref="WidgetList"/> in a grid pattern. </summary>
    public static class GridWriter
    {
        public static void InsertFiller(Widget widget, int width, int height, Widget filler)
        {
            while (widget.Children.Count < width * height) widget.Add((Widget)filler.Clone());
        }

        public static void Align(WidgetList widgets, int width, int height, RectangleF new_area, Point2? spacing = null, bool debug = false)
        {
            if (width == 0 || height == 0) return;

            // I am trying to get away with this
            if (spacing.HasValue) new_area.Size = new_area.Size - new Size2(spacing.Value.X * (width + 1), spacing.Value.Y * (height + 1));
            SetSize(widgets, width, height, new_area.Size, debug); // expand
            AutoSizeAllWidgets(widgets, width, height); // match follows
            AutoSpaceAllWidgets(widgets, width, height, new_area.Position, spacing); 
        }

        public static void AddRow(WidgetList widgets, int width, int height, int row, IEnumerable<Widget> new_row)
        {
            int start = row * width;
            int i = 0;
            foreach (Widget widget in new_row)
            {
                widgets.Insert(start + i++, widget);
            }
        }

        public static void AddColumn(WidgetList widgets, int width, int height, int column, IEnumerable<Widget> new_column)
        {
            int i = 0;
            foreach (Widget widget in new_column)
            {
                widgets.Insert(column + i++ * width, widget);
            }
        }

        public static void RemoveRow(WidgetList widgets, int width, int height, int row)
        {
            for (int i = 0; i < width; i++)
            {
                widgets.RemoveAt(row * width);
            }
        }

        public static void RemoveColumn(WidgetList widgets, int width, int height, int column)
        {
            for (int i = height -1; i >= 0; i--)
            {
                widgets.RemoveAt(column + i * width);
            }
        }

        /// <summary> This will find the longest/tallest widget in each row/collumn and make every other element match. </summary>
        public static void AutoSizeAllWidgets(WidgetList widgets, int width, int height)
        {
            for (int x = 0; x < width; x++) GridReader.GetColumn(widgets, width, height, x).AutoSizeWidth();
            for (int y = 0; y < height; y++) GridReader.GetRow(widgets, width, y).AutoSizeHeight();
        }

        public static void AutoSpaceAllWidgets(WidgetList widgets, int width, int height, Point2 start, Point2? spacing = null)
        {
            if (spacing.HasValue) start = start.WithOffset(spacing.Value);
            Point2 position = start;

            for (int x = 0; x < width; x++)
            {
                position.Y = start.Y;
                for (int y = 0; y < height; y++)
                {
                    Widget widget = GridReader.Get(widgets, width, height, x, y);
                    widget.Position = position;
                    position.Y += widget.Height;
                    if (spacing.HasValue) position.Y += spacing.Value.Y;
                }

                position.X += widgets[x].Width;
                if (spacing.HasValue) position.X += spacing.Value.X;
            }
        }

        private static void SetSize(WidgetList widgets, int width, int height, Point2 new_size, bool debug = false) {
            Point2 original_size = new Point2(GridReader.GetRow(widgets, width, 0).CombinedWidth, GridReader.GetColumn(widgets, width, height, 0).CombinedHeight);
            Point2 fixed_size = GridReader.FixedContentSizeTotal(widgets, width, height);
            Point2 modifier = new_size.DividedBy(original_size.WithOffset(fixed_size.Inverted()).WithMinValue(0.0001f));
            widgets.ExpandAll(modifier);
        }
    }
}
