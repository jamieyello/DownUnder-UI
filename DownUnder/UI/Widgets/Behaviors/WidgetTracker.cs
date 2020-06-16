using DownUnder.UI.Widgets.DataTypes;

namespace DownUnder.UI.Widgets.Behaviors
{
    /// <summary> Used by <see cref="WidgetBehavior"/>s to keep track of <see cref="Widgets.Widget"/>s. Call <see cref="Forget"/> on discarding to avoid tag clutter. </summary>
    public class WidgetTracker
    {
        private readonly WidgetBehavior _behavior;
        private Widget _widget;
        private readonly string _key;
        private readonly string _value;

        public WidgetTracker(WidgetBehavior behavior, string key, string value)
        {
            _behavior = behavior;
            _key = key;
            _value = value;
        }

        public Widget Widget
        {
            get => _widget;
            set
            {
                if (_widget == value) return;
                if (_widget != null)
                {
                    _behavior.RemoveTag(_widget, _key);
                    _behavior.Parent.Remove(_widget);
                }
                _widget = value;
                if (value == null) return;
                _behavior.SetTag(value, _key, _value);
                _behavior.Parent.Add(value);
            }
        }

        public bool FindIn(WidgetList widgets)
        {
            for (int i = 0; i < widgets.Count; i++)
            {
                if (widgets[i].BehaviorTags.TryGetValue(_behavior.GetType(), out var tags) && tags.TryGetValue(_key, out string value) && value == _value)
                {
                    Widget = widgets[i];
                    return true;
                }
            }

            return false;
        }

        public void Forget()
        {
            Widget = null;
        }
    }
}
