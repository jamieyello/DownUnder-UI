using System;

namespace DownUnder.UI.Widgets.DataTypes {
    /// <summary> Used to determine which slots to invoke. </summary>
    public enum FocusType {
        hover,
        selection
    }

    /// <summary> Keeps track of widgets that are focused by either hover or selection. </summary>
    public sealed class Focus : ICloneable {
        readonly WidgetList _focused_widgets = new WidgetList();
        readonly FocusType _focus_type;

        public Widget Primary => _focused_widgets.Count > 0 ? _focused_widgets[^1] : null;

        public Focus(
            FocusType focus_type
        ) =>
            _focus_type = focus_type;

        public Focus(
            Focus source
        ) : this(source._focus_type) =>
            _focused_widgets.AddRange(source._focused_widgets);

        public void AddFocus(Widget widget) {
            if (_focused_widgets.Contains(widget))
                return;

            _focused_widgets.Add(widget);

            if (_focus_type == FocusType.selection)
                widget.InvokeSelectEvent();
        }

        public void UnFocus(Widget widget) {
            if (!_focused_widgets.Contains(widget))
                return;

            _focused_widgets.Remove(widget);

            if (_focus_type == FocusType.selection)
                widget.InvokeSelectOffEvent();
        }

        public void SetFocus(Widget widget) {
            if (_focused_widgets.Count == 1 && _focused_widgets[0] == widget)
                return; // Return if this is the only selected widget

            if (_focused_widgets.Contains(widget)) {
                // Remove all but given widget and trigger their lose focus events if other widgets are focused
                for (var i = _focused_widgets.Count - 1; i >= 0; i--) {
                    var focus_off_widget = _focused_widgets[i];
                    if (focus_off_widget != widget)
                        UnFocus(focus_off_widget);
                }
                return;
            }

            Reset();
            AddFocus(widget);
        }

        public void Reset() {
            if (_focus_type == FocusType.selection)
                foreach (var widget in _focused_widgets)
                    widget.InvokeSelectOffEvent();

            _focused_widgets.Clear();
        }

        public bool IsWidgetFocused(Widget widget) =>
            _focused_widgets.Contains(widget);

        /// <summary> Returns a shallow clone of the focused <see cref="Widget"/>s. </summary>
        public WidgetList ToWidgetList() => new WidgetList(null, _focused_widgets, true);

        object ICloneable.Clone() => Clone();
        public Focus Clone() => new Focus(this);
    }
}