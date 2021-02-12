using System.Collections.Generic;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.DataTypes {
    /// <summary> Allows aligning a <see cref="WidgetList"/> in a grid pattern. </summary>
    public static class GridWriter {
        public static void InsertFiller(
            Widget widget,
            int width,
            int height,
            Widget filler
        ) {
            var area = width * height;
            while (widget.Children.Count < area)
                widget.Add((Widget)filler.Clone());
        }

        public static void Align(
            WidgetList widgets,
            int width,
            int height,
            RectangleF new_area,
            Point2? spacing = null,
            bool debug = false
        ) {
            if (width == 0 || height == 0)
                return;

            AutoSizeAllWidgets(widgets, width, height);
            SetSize(widgets, width, height, new_area.Size, debug, spacing); // expand
            AutoSpaceAllWidgets(widgets, width, height, new_area.Position, spacing);
        }

        public static void AddRow(
            WidgetList widgets,
            int width,
            int height,
            int row,
            IEnumerable<Widget> new_row
        ) {
            var start = row * width;
            var i = 0;

            foreach (var widget in new_row)
                widgets.Insert(start + i++, widget);
        }

        public static void AddColumn(
            WidgetList widgets,
            int width,
            int height,
            int column,
            IEnumerable<Widget> new_column
        ) {
            var i = 0;
            foreach (var widget in new_column)
                widgets.Insert(column + i++ * width, widget);
        }

        public static void RemoveRow(
            WidgetList widgets,
            int width,
            int height,
            int row
        ) {
            for (var i = 0; i < width; i++)
                widgets.RemoveAt(row * width);
        }

        public static void RemoveColumn(
            WidgetList widgets,
            int width,
            int height,
            int column
        ) {
            for (var i = height - 1; i >= 0; i--)
                widgets.RemoveAt(column + i * width);
        }

        /// <summary> This will find the longest/tallest widget in each row/collumn and make every other element match. </summary>
        public static void AutoSizeAllWidgets(
            WidgetList widgets,
            int width,
            int height
        ) {
            for (var x = 0; x < width; x++)
                GridReader.GetColumn(widgets, width, height, x).AutoSizeWidth();

            for (var y = 0; y < height; y++)
                GridReader.GetRow(widgets, width, y).AutoSizeHeight();
        }

        public static void AutoSpaceAllWidgets(
            WidgetList widgets,
            int width,
            int height,
            Point2 start,
            Point2? spacing = null
        ) {
            if (spacing.HasValue)
                start = start.WithOffset(spacing.Value);

            var position = start;

            for (var x = 0; x < width; x++) {
                position.Y = start.Y;
                for (var y = 0; y < height; y++) {
                    var widget = GridReader.Get(widgets, width, height, x, y);
                    widget.Position = position;
                    position.Y += widget.Height;
                    if (spacing.HasValue)
                        position.Y += spacing.Value.Y;
                }

                position.X += widgets[x].Width;
                if (spacing.HasValue)
                    position.X += spacing.Value.X;
            }
        }

        static void SetSize(
            WidgetList widgets,
            int width,
            int height,
            Point2 new_size,
            bool debug = false,
            Point2? spacing = null // TODO: what does null spacing mean? is it different from zero spacing?
        ) {
            if (spacing.HasValue)
                new_size -= TotalSpacingOffset(width, height, spacing.Value);

            var original_size = new Point2(
                GridReader.GetRow(widgets, width, 0).CombinedWidth,
                GridReader.GetColumn(widgets, width, height, 0).CombinedHeight
            );

            var fixed_size = GridReader.FixedContentSizeTotal(widgets, width, height);

            var total_offset = TotalSpacingOffset(width, height, spacing ?? new Point2());
            var fixed_with_offset = fixed_size.Inverted().WithOffset(total_offset);
            var original_with_offset = original_size.WithOffset(fixed_with_offset).WithMinValue(0.0001f);
            var modifier = new_size.DividedBy(original_with_offset);

            widgets.ExpandAll(modifier);
        }

        static Size2 TotalSpacingOffset(float width, float height, Point2 spacing) =>
            new Size2(spacing.X * (width + 1), spacing.Y * (height + 1));
    }
}

