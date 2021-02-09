using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DownUnder.UI.UI.Widgets.Actions;
using DownUnder.UI.UI.Widgets.Actions.Functional;
using DownUnder.UI.UI.Widgets.CustomEventArgs;
using DownUnder.UI.Utilities.CommonNamespace;
using DownUnder.UI.Utilities.Extensions;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.DataTypes {
    /// <summary> A class to make interfacing with a List<Widget> easier. </summary>
    [DataContract]
    public sealed class WidgetList : IEnumerable<Widget> {
        Widget _parent;
        [DataMember] List<Widget> _widgets = new List<Widget>();

        public Widget LastAddedWidget { get; private set; }
        public Widget LastRemovedWidget { get; private set; }

        [DataMember]
        public Widget Parent {
            get => _parent;
            set {
                _parent = value;
                foreach (var widget in _widgets)
                    widget.Parent = _parent;
            }
        }

        public WidgetList() =>
            IsReadOnly = false;

        public WidgetList(Widget parent) =>
            Parent = parent;

        public WidgetList(
            bool is_read_only = false
        ) =>
            IsReadOnly = is_read_only;

        public WidgetList(
            Widget parent,
            IEnumerable<Widget> widgets,
            bool is_read_only = false
        ) {
            _parent = parent;
            foreach (var widget in widgets)
                _widgets.Add(widget);
            IsReadOnly = is_read_only;
        }

        [OnDeserializing]
        void OnDeserialize(StreamingContext context) =>
            _widgets = new List<Widget>();

        void OnRemove(Widget removed) {
            Parent?.InvokeOnRemove();
            Parent?.InvokeOnRemoveAny(new WidgetArgs(removed));
        }

        void OnAdd(Widget added) {
            Parent?.InvokeOnAdd();
            Parent?.InvokeOnAddAny(new WidgetArgs(added));
        }

        void OnListChange() =>
            Parent?.InvokeOnListChange();

        public List<RectangleF> GetHorizontalWrapAreas(float width, float spacing) {
            // Read the areas of the widgets just once (areas)

            var areas =
                _widgets
                .Select(widget => widget.Area)
                .ToList();

            var new_areas = areas.ToList();
            var max_height = areas.Select(area => area.Height).Append(0f).Max();
            var max_width = areas.Select(area => area.Width).Append(0f).Max();

            var widgets_per_line = (int)(width / (max_width + spacing));

            if (widgets_per_line <= 0)
                widgets_per_line = 1;

            var x_spacing = (width - max_width * widgets_per_line) / (widgets_per_line + 1f);

            for (var i = 0; i < _widgets.Count; i++) {
                var x = i % widgets_per_line;

                new_areas[i] = new RectangleF(
                    x * x_spacing + x * max_width + spacing,
                    i / widgets_per_line * (max_height + spacing) + spacing,
                    areas[i].Width,
                    areas[i].Height
                );
            }

            return new_areas;
        }

        public void SetAreas(
            List<RectangleF> areas,
            InterpolationSettings? interpolation = null
        ) {
            if (interpolation == null)
                for (var i = 0; i < areas.Count; i++)
                    _widgets[i].Area = areas[i];
            else {
                for (var i = 0; i < areas.Count; i++) {
                    var action = new PropertyTransitionAction<RectangleF>(nameof(Widget.Area), areas[i], interpolation) {
                        DuplicatePolicy = WidgetAction.DuplicatePolicyType.@override,
                        DuplicateDefinition = WidgetAction.DuplicateDefinitionType.interferes_with
                    };

                    _widgets[i].Actions.Add(action);
                }
            }
        }

        /// <summary> Returns the <see cref="Point2.Max"/> result of all <see cref="Widget"/>'s sizes. </summary>
        public Point2 SizeMax { get {
            var max_size = new Point2();
            foreach (var widget in _widgets)
                max_size = max_size.Max(widget.Size);
            return max_size;
        } }

        /// <summary> Returns the <see cref="Point2.Min"/> result of all <see cref="Widget"/>'s sizes. </summary>
        public Point2 SizeMin { get {
            if (_widgets.Count == 0)
                return new Point2();

            var min_size = _widgets[0].Size;
            for (var i = 1; i < _widgets.Count; i++)
                min_size = min_size.Min(_widgets[i].Size);
            return min_size;
        } }

        /// <summary> Get the maximum allowed <see cref="Widget.MinimumSize"/> for this group of <see cref="Widget"/>s. </summary>
        public Point2 MinimumWidgetSize { get {
            var min_size = new Point2();
            for (var i = 0; i < _widgets.Count; i++)
                min_size = min_size.Max(_widgets[i].MinimumSize);
            return min_size;
        } }

        /// <summary> Get the maximum allowed <see cref="Widget.MinimumWidth"/> for this group of <see cref="Widget"/>s. </summary>
        public float MinimumWidgetWidth { get {
            var min_width = 0f;
            for (var i = 0; i < _widgets.Count; i++)
                min_width = Math.Max(min_width, _widgets[i].MinimumWidth);
            return min_width;
        } }

        /// <summary> Get the maximum allowed <see cref="Widget.MinimumHeight"/> for this group of <see cref="Widget"/>s. </summary>
        public float MinimumWidgetHeight { get {
            var min_height = 0f;
            for (var i = 0; i < _widgets.Count; i++)
                min_height = Math.Max(min_height, _widgets[i].MinimumHeight);
            return min_height;
        } }

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

        public RectangleF? AreaCoverage { get {
            if (Count == 0)
                return null;

            var result = _widgets[0].Area;
            for (var i = 1; i < _widgets.Count; i++)
                result = result.Union(_widgets[i].Area);
            return result;
        } }

        public float CombinedHeight { get {
            var result = 0f;
            foreach (var widget in _widgets)
                result += widget.Height;
            return result;
        } }

        public float CombinedWidth { get {
            var result = 0f;
            foreach (var widget in _widgets)
                result += widget.Width;
            return result;
        } }

        public void SetParent(IParent parent) {
            foreach (var widget in _widgets)
                widget.Parent = parent;
        }

        public void ExpandAll(float modifier) {
            foreach (var widget in _widgets)
                widget.Area = widget.Area.WithScaledSize(modifier);
        }

        public void ExpandAll(Point2 modifier) {
            foreach (var widget in _widgets)
                widget.Area = widget.Area.WithScaledSize(modifier);
        }

        public void ResizeBy(
            BorderSize border_size,
            bool uniform_minimum_size = false
        ) {
            foreach (var w in _widgets)
                w.Area = w.Area.ResizedBy(border_size, uniform_minimum_size ? MinimumWidgetSize : (Point2?)null);
        }

        public void ResizeBy(
            float amount,
            Directions2D directions,
            bool uniform_minimum_size = false
        ) =>
            ResizeBy(new BorderSize(amount, directions), uniform_minimum_size);

        /// <summary> Set all the widget's width values to match each other. </summary>
        public void AutoSizeWidth() {
            var new_width = SizeMax.X;

            foreach (var widget in _widgets) {
                if (widget.IsFixedWidth) {
                    new_width = widget.Width;
                    break;
                }
            }

            SetAllWidth(new_width);
        }

        /// <summary> Set all the widget's height values to match each other. </summary>
        public void AutoSizeHeight() {
            var new_height = SizeMax.Y;

            foreach (var widget in _widgets) {
                if (widget.IsFixedHeight) {
                    new_height = widget.Height;
                    break;
                }
            }

            SetAllHeight(new_height);
        }

        public void SetAllWidth(float width) {
            foreach (var widget in _widgets)
                widget.Width = width;
        }

        public void SetAllHeight(float height) {
            foreach (var widget in _widgets)
                widget.Height = height;
        }

        public void SendWidgetToBack(Widget widget) {
            var index = IndexOf(widget);
            if (index == -1)
                throw new Exception($"{nameof(Widget)} not found.");

            _widgets.RemoveAt(index);
            _widgets.Insert(0, widget);
        }

        public void SendWidgetToFront(Widget widget) {
            var index = IndexOf(widget);
            if (index == -1)
                throw new Exception($"{nameof(Widget)} not found.");

            _widgets.RemoveAt(index);
            _widgets.Add(widget);
        }

        void ThrowReadOnlyException() =>
            throw new Exception($"This {GetType().Name} is read only.");

        #region IList Implementation

        public Widget this[int index] {
            get => _widgets[index];
            set {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        public Widget this[string name] { get {
            for (var i = 0; i < _widgets[i].Count; i++)
                if (_widgets[i].Name == name)
                    return _widgets[i];

            return null;
        } }

        public int Count => _widgets.Count;
        public bool IsReadOnly { get; }

        public static implicit operator List<Widget>(WidgetList v) => new List<Widget>(v);
        public static implicit operator Widget[](WidgetList v) => v._widgets.ToArray();
        public void Add(Widget widget) => Insert(Count, widget);

        public void Clear() {
            if (IsReadOnly)
                ThrowReadOnlyException();

            for (var i = _widgets.Count - 1; i >= 0; i--)
                RemoveAt(i);
        }

        public bool Contains(Widget widget) => _widgets.Contains(widget);
        public void CopyTo(Widget[] array, int array_index) => _widgets.CopyTo(array, array_index);
        public IEnumerator<Widget> GetEnumerator() => _widgets.GetEnumerator();
        public int IndexOf(Widget widget) => _widgets.IndexOf(widget);

        public void Insert(int index, Widget widget) {
            if (IsReadOnly)
                ThrowReadOnlyException();

            if (_parent != null)
                widget.Parent = _parent;

            _widgets.Insert(index, widget);
            LastAddedWidget = widget;
            OnAdd(widget);
            OnListChange();
        }

        public bool Remove(Widget widget) {
            if (IsReadOnly)
                ThrowReadOnlyException();

            if (!_widgets.Remove(widget))
                return false;

            if (_parent is { })
                widget.Parent = null;

            LastRemovedWidget = widget;
            OnRemove(widget);
            OnListChange();
            return true;
        }

        public void RemoveAt(int index) {
            if (IsReadOnly)
                ThrowReadOnlyException();

            LastRemovedWidget = _widgets[index];

            if (_parent is { })
                _widgets[index].Parent = null;

            _widgets.RemoveAt(index);
            OnRemove(LastRemovedWidget);
            OnListChange();
        }

        IEnumerator IEnumerable.GetEnumerator() => _widgets.GetEnumerator();

        public void AddRange(IEnumerable<Widget> widgets) {
            if (IsReadOnly)
                ThrowReadOnlyException();

            foreach (var widget in widgets)
                Add(widget);
        }

        public WidgetList GetRange(int index, int count) =>
            new WidgetList(null, _widgets.GetRange(index, count));

        #endregion
    }
}