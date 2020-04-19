using DownUnder.UI.Widgets.Interfaces;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DownUnder.UI.Widgets.DataTypes
{
    /// <summary> A class to make interfacing with a List<Widget> easier. </summary>
    public class WidgetList : IList<Widget>
    {
        private List<Widget> _widgets { get; set; } = new List<Widget>();

        public WidgetList() { }
        public WidgetList(List<Widget> widget_list)
        {
            foreach (Widget widget in widget_list)
            {
                ((IList<Widget>)_widgets).Add(widget);
            }

            Count = _widgets.Count;
        }

        public void AlignHorizontalWrap(float max_width, bool debug_output = false, float spacing = 0f, bool consistent_row_height = true)
        {
            if (_widgets.Count == 0) return;
            max_width = max_width - spacing;

            // Read the areas of the widgets just once (areas)
            List<RectangleF> areas = new List<RectangleF>();
            foreach (Widget widget in _widgets)
            {
                areas.Add(widget.Area);
            }

            // Determine the number of widgets that should be in a row (row_x_count)
            Point2 point;
            int row_x_count = areas.Count;
            point = new Point2(spacing, spacing);
            int this_row_count = 0;

            for (int i = 0; i < areas.Count; i++)
            {
                point.X += areas[i].Width + spacing;
                this_row_count++;
                if (point.X > max_width)
                {
                    row_x_count = Math.Min(this_row_count, row_x_count);
                    this_row_count = 0;
                    point.X = spacing * 2;
                }
            }

            // Given the
            // row_x_count
            // max_width
            // row_height
            // Set the positions of the Widgets.
            float row_height = MaxSize.Y + spacing;
            point = new Point2(spacing, spacing);
            int x = 0;
            for (int i = 0; i < _widgets.Count; i++)
            {
                point.X = max_width * ((float)(x) / (row_x_count)) + spacing;
                _widgets[i].Area = areas[i].WithPosition(point);

                if (++x == row_x_count)
                {
                    x = 0;
                    point.Y += row_height;
                }
            }

            // Debug because this is still broken
            if (false)
            {
                for (int i = 0; i < areas.Count; i++)
                {
                    Console.WriteLine($"areas[{i}] {areas[i]}");

                }
                Console.WriteLine($"row_x_count {row_x_count}");
                Console.WriteLine($"max_width {max_width}");
            }
        }

        public Point2 MaxSize {
            get {
                Point2 max_size = new Point2();
                foreach (Widget widget in _widgets) max_size = max_size.Max(widget.Size);
                return max_size;
            }
        }

        public RectangleF? AreaCoverage {
            get {
                if (Count == 0) return null;
                RectangleF result = _widgets[0].Area;
                for (int i = 1; i < _widgets.Count; i++) result = result.Union(_widgets[i].Area);
                return result;
            }
        }

        public float CombinedHeight
        {
            get
            {
                float result = 0f;
                foreach (Widget widget in _widgets)
                {
                    result += widget.Height;
                }
                return result;
            }
        }

        public float CombinedWidth
        {
            get
            {
                float result = 0f;
                foreach (Widget widget in _widgets)
                {
                    result += widget.Width;
                }
                return result;
            }
        }

        public void SetParent(IParent parent) {
            foreach (Widget widget in _widgets) widget.Parent = parent;
        }

        public void ExpandAll(float modifier) {
            foreach (Widget widget in _widgets) widget.Area = widget.Area.Resized(modifier);
        }

        public void ExpandAll(Point2 modifier) {
            foreach (Widget widget in _widgets) widget.Area = widget.Area.Resized(modifier);
        }

        /// <summary> Set all the widget's width values to match each other. </summary>
        public void AutoSizeWidth() {
            float new_width = MaxSize.X;
            
            foreach (Widget widget in _widgets) {
                if (widget.IsFixedWidth) {
                    new_width = widget.Width;
                    break;
                }
            }

            SetAllWidth(new_width);
        }

        /// <summary> Set all the widget's height values to match each other. </summary>
        public void AutoSizeHeight() {
            float new_height = MaxSize.Y;

            foreach (Widget widget in _widgets) {
                if (widget.IsFixedHeight) {
                    new_height = widget.Height;
                    break;
                }
            }

            SetAllHeight(new_height);
        }

        public void SetAllWidth(float width) {
            foreach (Widget widget in _widgets) widget.Width = width;
        }

        public void SetAllHeight(float height) {
            foreach (Widget widget in _widgets) widget.Height = height;
        }

        #region IList Implementation

        public Widget this[int index]
        {
            get => ((IList<Widget>)_widgets)[index];
            set => ((IList<Widget>)_widgets)[index] = value;
        }

        public int Count { get; private set; } = 0;

        public bool IsReadOnly => ((IList<Widget>)_widgets).IsReadOnly;
        
        public static implicit operator List<Widget>(WidgetList v)
        {
            return v._widgets;
        }

        public void Add(Widget widget) {
            ((IList<Widget>)_widgets).Add(widget);
            Count = _widgets.Count;
        }

        public void Clear() {
            ((IList<Widget>)_widgets).Clear();
            Count = 0;
        }

        public bool Contains(Widget widget) => ((IList<Widget>)_widgets).Contains(widget);
        public void CopyTo(Widget[] array, int array_index) => ((IList<Widget>)_widgets).CopyTo(array, array_index);
        public IEnumerator<Widget> GetEnumerator() => ((IList<Widget>)_widgets).GetEnumerator();
        public int IndexOf(Widget widget) => ((IList<Widget>)_widgets).IndexOf(widget);

        public void Insert(int index, Widget widget) {
            ((IList<Widget>)_widgets).Insert(index, widget);
            Count = _widgets.Count;
        }

        public bool Remove(Widget widget) {
            if (((IList<Widget>)_widgets).Remove(widget)) {
                Count = _widgets.Count;
                return true;
            }
            return false;
        }

        public void RemoveAt(int index) {
            ((IList<Widget>)_widgets).RemoveAt(index);
            Count = _widgets.Count;
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IList<Widget>)_widgets).GetEnumerator();

        public void AddRange(WidgetList allContainedWidgets) {
            _widgets.AddRange(allContainedWidgets);
            Count = _widgets.Count;
        }

        #endregion
    }
}
