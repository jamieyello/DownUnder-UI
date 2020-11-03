using DownUnder.UI.Widgets.Actions;
using DownUnder.UI.Widgets.Actions.Functional;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utilities;
using DownUnder.Utility;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.DataTypes
{
    /// <summary> A class to make interfacing with a List<Widget> easier. </summary>
    [DataContract] 
    public class WidgetList : IList<Widget>, IIsWidgetChild
    {
        Widget _parent;

        [DataMember] private List<Widget> _widgets = new List<Widget>();

        public Widget LastAddedWidget;
        public Widget LastRemovedWidget;

        public Widget Parent 
        { 
            get => _parent;
            set
            {
                _parent = value;
                foreach (Widget widget in _widgets) widget.Parent = _parent;
            } 
        }

        public WidgetList() { IsReadOnly = false; }
        public WidgetList(Widget parent) { Parent = parent; }
        public WidgetList(bool is_read_only = false) { IsReadOnly = is_read_only; }
        public WidgetList(Widget parent, List<Widget> widget_list, bool is_read_only = false) {
            _parent = parent;
            foreach (Widget widget in widget_list) ((IList<Widget>)_widgets).Add(widget);
            IsReadOnly = is_read_only;

            Count = _widgets.Count;
        }

        private void OnRemove() { Parent?.InvokeOnRemove(); }
        private void OnAdd() { Parent?.InvokeOnAdd(); }
        private void OnListChange() { Parent?.InvokeOnListChange(); }

        public List<RectangleF> GetHorizontalWrapAreas(float width, float spacing)
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

            for (int i = 0; i < _widgets.Count; i++)
            {
                int x = (i % widgets_per_line);
                new_areas[i] = new RectangleF(
                    x * x_spacing + x * max_width + spacing,
                    i / widgets_per_line * (max_height + spacing) + spacing,
                    areas[i].Width,
                    areas[i].Height);
            }

            return new_areas;
        }

        public void SetAreas(List<RectangleF> areas, InterpolationSettings? interpolation = null)
        {
            if (interpolation == null) {
                for (int i = 0; i < areas.Count; i++) _widgets[i].Area = areas[i];
            }
            else {
                for (int i = 0; i < areas.Count; i++) _widgets[i].Actions.Add(new PropertyTransitionAction<RectangleF>(nameof(Widget.Area), areas[i], interpolation) 
                { 
                    DuplicatePolicy = WidgetAction.DuplicatePolicyType.@override 
                    , DuplicateDefinition = WidgetAction.DuplicateDefinitionType.interferes_with
                });
            }
        }

        /// <summary> Returns the <see cref="Point2.Max"/> result of all <see cref="Widget"/>'s sizes. </summary>
        public Point2 SizeMax {
            get {
                Point2 max_size = new Point2();
                foreach (Widget widget in _widgets) max_size = max_size.Max(widget.Size);
                return max_size;
            }
        }

        /// <summary> Returns the <see cref="Point2.Min"/> result of all <see cref="Widget"/>'s sizes. </summary>
        public Point2 SizeMin {
            get {
                if (_widgets.Count == 0) return new Point2();
                Point2 min_size = _widgets[0].Size;
                for (int i = 1; i < _widgets.Count; i++) min_size = min_size.Min(_widgets[i].Size);
                return min_size;
            }
        }

        /// <summary> Get the maximum allowed <see cref="Widget.MinimumSize"/> for this group of <see cref="Widget"/>s. </summary>
        public Point2 MinimumWidgetSize
        {
            get {
                Point2 min_size = new Point2();
                for (int i = 0; i < _widgets.Count; i++) min_size = min_size.Max(_widgets[i].MinimumSize);
                return min_size;
            }
        }

        /// <summary> Get the maximum allowed <see cref="Widget.MinimumWidth"/> for this group of <see cref="Widget"/>s. </summary>
        public float MinimumWidgetWidth {
            get {
                float min_width = 0f;
                for (int i = 0; i < _widgets.Count; i++) min_width = Math.Max(min_width, _widgets[i].MinimumWidth);
                return min_width;
            }
        }

        /// <summary> Get the maximum allowed <see cref="Widget.MinimumHeight"/> for this group of <see cref="Widget"/>s. </summary>
        public float MinimumWidgetHeight {
            get {
                float min_height = 0f;
                for (int i = 0; i < _widgets.Count; i++) min_height = Math.Max(min_height, _widgets[i].MinimumHeight);
                return min_height;
            }
        }

        // Widget doesn't implement max size yet
        //public Point2 MaximumWidgetSize
        //{
        //    get
        //    {
        //        if (_widgets.Count == 0) return new Point2();
        //        Point2 min_size = _widgets[0].M;
        //        for (int i = 1; i < _widgets.Count; i++) min_size = min_size.Max(_widgets[i].MinimumSize);
        //        return max_size;
        //    }
        //}

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
            foreach (Widget widget in _widgets) widget.Area = widget.Area.WithScaledSize(modifier);
        }

        public void ExpandAll(Point2 modifier) {
            foreach (Widget widget in _widgets) widget.Area = widget.Area.WithScaledSize(modifier);
        }

        public void ResizeBy(BorderSize border_size, bool uniform_minimum_size = false) {
            if (uniform_minimum_size) {
                for (int i = 0; i < _widgets.Count; i++) _widgets[i].Area = _widgets[i].Area.ResizedBy(border_size, MinimumWidgetSize);
            }
            else {
                for (int i = 0; i < _widgets.Count; i++) _widgets[i].Area = _widgets[i].Area.ResizedBy(border_size);
            }
        }

        public void ResizeBy(float amount, Directions2D directions, bool uniform_minimum_size = false) =>
            ResizeBy(new BorderSize(amount, directions), uniform_minimum_size);

        /// <summary> Set all the widget's width values to match each other. </summary>
        public void AutoSizeWidth() {
            float new_width = SizeMax.X;
            
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
            float new_height = SizeMax.Y;

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
                OnRemove();
                OnAdd();
                OnListChange();
            }
        }

        public int Count { get; private set; } = 0;

        public bool IsReadOnly { get; }

        public static implicit operator List<Widget>(WidgetList v) => new List<Widget>(v);
        public static implicit operator Widget[](WidgetList v) => v._widgets.ToArray();

        public void Add(Widget widget) {
            if (IsReadOnly) ThrowReadOnlyException();
            if (_parent != null) widget.Parent = _parent;
            _widgets.Add(widget);
            LastAddedWidget = widget;
            OnAdd();
            OnListChange();
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
            if (_parent != null) widget.Parent = _parent;
            _widgets.Insert(index, widget);
            Count = _widgets.Count;
            LastAddedWidget = widget;
            OnAdd();
            OnListChange();
        }

        public bool Remove(Widget widget) {
            if (IsReadOnly) ThrowReadOnlyException();
            if (_widgets.Remove(widget)) {
                if (_parent != null) widget.Parent = null;
                Count = _widgets.Count;
                LastRemovedWidget = widget;
                OnRemove();
                OnListChange();
                return true;
            }
            return false;
        }

        public void RemoveAt(int index) {
            if (IsReadOnly) ThrowReadOnlyException();
            LastRemovedWidget = _widgets[index];
            if (_parent != null) _widgets[index].Parent = null;
            _widgets.RemoveAt(index);
            Count = _widgets.Count;
            OnRemove();
            OnListChange();
        }

        IEnumerator IEnumerable.GetEnumerator() => _widgets.GetEnumerator();

        public void AddRange(IEnumerable<Widget> widgets) {
            if (IsReadOnly) ThrowReadOnlyException();
            foreach (Widget widget in widgets)
            {
                Add(widget);
            }
        }

        public WidgetList GetRange(int index, int count) => new WidgetList(null, _widgets.GetRange(index, count));

        #endregion
    }
}
