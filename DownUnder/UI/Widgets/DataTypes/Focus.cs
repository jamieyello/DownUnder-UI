namespace DownUnder.UI.Widgets.DataTypes
{
    /// <summary> Used to determine which slots to invoke. </summary>
    public enum FocusType {
        hover,
        selection
    }

    /// <summary> Keeps track of widgets that are focused by either hover or selection. </summary>
    public class Focus
    {
        private WidgetList _focused_widgets = new WidgetList();
        private readonly FocusType _focus_type;

        public Focus(FocusType focus_type) {
            _focus_type = focus_type;
        }

        public void AddFocus(Widget widget) {
            if (_focused_widgets.Contains(widget)) return;
            _focused_widgets.Add(widget);
            if (_focus_type == FocusType.selection) widget.TriggerSelectEvent();
        }

        public void UnFocus(Widget widget) {
            if (!_focused_widgets.Contains(widget)) return;
            _focused_widgets.Remove(widget);
            if (_focus_type == FocusType.selection) widget.TriggerSelectOffEvent();
        }

        public void SetFocus(Widget widget) {
            Reset();
            _focused_widgets.Add(widget);
            widget.TriggerSelectEvent();
        }

        public void Reset() {
            if (_focus_type == FocusType.selection) foreach (Widget widget in _focused_widgets) widget.TriggerSelectOffEvent();
            _focused_widgets.Clear();
        }

        public bool IsWidgetFocused(Widget widget) => _focused_widgets.Contains(widget);

        public Widget Primary => _focused_widgets.Count > 0 ? _focused_widgets[_focused_widgets.Count - 1] : null;

        public WidgetList ToWidgetList() => _focused_widgets;
    }
}