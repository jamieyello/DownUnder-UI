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
        private readonly List<Widget> _widgets = new List<Widget>();

        public Widget LastAddedWidget;
        public Widget LastRemovedWidget;

        /// <summary> Invoked whenever a widget is added or removed. </summary>
        public event EventHandler OnAdd;
        public event EventHandler OnRemove;
        public event EventHandler OnListChange;

        public WidgetList() { IsReadOnly = false; }
        public WidgetList(bool is_read_only = false) { IsReadOnly = is_read_only; }
        public WidgetList(List<Widget> widget_list, bool is_read_only = false) {
            foreach (Widget widget in widget_list) ((IList<Widget>)_widgets).Add(widget);
            IsReadOnly = is_read_only;

            Count = _widgets.Count;
        }

        public void AlignHorizontalWrap(float width, float spacing)
        {
            // Read the areas of the widgets just once (areas)
            List<RectangleF> areas = new List<RectangleF>();
            List<RectangleF> new_areas = new List<RectangleF>();
            foreach (Widget widget in _widgets) areas.Add(widget.Area);
            foreach (RectangleF area in areas) new_areas.Add(area);
            float max_height = 0;
            float max_width = 0;

            foreach (RectangleF area in areas) max_height = Math.Max(max_height, area.Height);
            foreach (RectangleF area in areas) max_width = Math.Max(max_width, area.Width);

            int widgets_per_line = (int)(width / (max_width + spacing));
            if (widgets_per_line <= 0) widgets_per_line = 1;

            float x_spacing = (width - max_width * widgets_per_line) / (widgets_per_line + 1);

            //Console.WriteLine(widgets_per_line);

            for (int i = 0; i < _widgets.Count; i++) 
            {
                int x = (i % widgets_per_line);
                new_areas[i] = new RectangleF(
                    x * x_spacing + x * max_width + spacing,
                    i / widgets_per_line * (max_height + spacing) + spacing,
                    areas[i].Width,
                    areas[i].Height);
            }

            for (int i = 0; i < new_areas.Count; i++) _widgets[i].Area = new_areas[i];
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

        public float CombinedHeight {
            get {
                float result = 0f;
                foreach (Widget widget in _widgets) result += widget.Height;
                return result;
            }
        }

        public float CombinedWidth {
            get {
                float result = 0f;
                foreach (Widget widget in _widgets) result += widget.Width;
                return result;
            }
        }

        public void SetParent(IParent parent) {
            foreach (Widget widget in _widgets) widget.Parent = parent;
        }

        public void ExpandAll(float modifier) {
            foreach (Widget widget in _widgets) widget.Area = widget.Area.ResizedBy(modifier);
        }

        public void ExpandAll(Point2 modifier) {
            foreach (Widget widget in _widgets) widget.Area = widget.Area.ResizedBy(modifier);
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

        private void ThrowReadOnlyException() => throw new Exception($"This {GetType().Name} is read only.");

        #region IList Implementation

        public Widget this[int index] {
            get => ((IList<Widget>)_widgets)[index];
            set {
                if (IsReadOnly) ThrowReadOnlyException();
                LastRemovedWidget = _widgets[index];
                LastAddedWidget = value;
                _widgets[index] = value;
                OnRemove.Invoke(this, EventArgs.Empty);
                OnAdd.Invoke(this, EventArgs.Empty);
                OnListChange.Invoke(this, EventArgs.Empty);
            }
        }

        public int Count { get; private set; } = 0;

        public bool IsReadOnly { get; }

        public static implicit operator List<Widget>(WidgetList v) => new List<Widget>(v);
        public static implicit operator Widget[](WidgetList v) => v._widgets.ToArray();

        public void Add(Widget widget) {
            if (IsReadOnly) ThrowReadOnlyException();
            _widgets.Add(widget);
            LastAddedWidget = widget;
            OnAdd?.Invoke(this, EventArgs.Empty);
            OnListChange?.Invoke(this, EventArgs.Empty);
            Count = _widgets.Count;
        }

        public void Clear() {
            if (IsReadOnly) ThrowReadOnlyException();
            for (int i = _widgets.Count - 1; i >= 0; i--) RemoveAt(i);
        }

        public bool Contains(Widget widget) => _widgets.Contains(widget);
        public void CopyTo(Widget[] array, int array_index) => _widgets.CopyTo(array, array_index);
        public IEnumerator<Widget> GetEnumerator() => _widgets.GetEnumerator();
        public int IndexOf(Widget widget) => _widgets.IndexOf(widget);

        public void Insert(int index, Widget widget) {
            if (IsReadOnly) ThrowReadOnlyException();
            _widgets.Insert(index, widget);
            Count = _widgets.Count;
            LastAddedWidget = widget;
            OnAdd?.Invoke(this, EventArgs.Empty);
            OnListChange?.Invoke(this, EventArgs.Empty);
        }

        public bool Remove(Widget widget) {
            if (IsReadOnly) ThrowReadOnlyException();
            if (_widgets.Remove(widget)) {
                Count = _widgets.Count;
                LastRemovedWidget = widget;
                OnRemove?.Invoke(this, EventArgs.Empty);
                OnListChange?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index) {
            if (IsReadOnly) ThrowReadOnlyException();
            LastRemovedWidget = _widgets[index];
            _widgets.RemoveAt(index);
            Count = _widgets.Count;
            OnRemove?.Invoke(this, EventArgs.Empty);
            OnListChange?.Invoke(this, EventArgs.Empty);
        }

        IEnumerator IEnumerable.GetEnumerator() => _widgets.GetEnumerator();

        public void AddRange(WidgetList widgets) {
            if (IsReadOnly) ThrowReadOnlyException();
            for (int i = 0; i < widgets.Count; i++) {
                Add(widgets[i]);
            }
        }

        public WidgetList GetRange(int index, int count) => new WidgetList(_widgets.GetRange(index, count));

        #endregion
    }
}
