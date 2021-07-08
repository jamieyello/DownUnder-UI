using DownUnder.UI.Widgets.DataTypes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DownUnder.UI.Widgets.Behaviors {
    /// <summary> Used by <see cref="WidgetBehavior"/>s to keep track of <see cref="Widgets.Widget"/>s. Call <see cref="Forget"/> on discarding to avoid cluttering used <see cref="Widget"/>s with tag information. </summary>
    public sealed class WidgetTracker {
        readonly WidgetBehavior _behavior;
        readonly string _key;
        readonly string _value;
        readonly Dictionary<string, EventHandler> _persistent_events = new Dictionary<string, EventHandler>();
        readonly Dictionary<string, EventHandler<RectangleFSetArgs>> _persistent_resize_events = new Dictionary<string, EventHandler<RectangleFSetArgs>>();
        readonly Dictionary<string, EventHandler<Point2SetArgs>> _persistent_point2set_events = new Dictionary<string, EventHandler<Point2SetArgs>>();
        readonly bool _use_tag;
        Widget _widget;

        public WidgetTracker(WidgetBehavior behavior) {
            _behavior = behavior;
            _use_tag = false;
        }

        public WidgetTracker(WidgetBehavior behavior, string key, string value) {
            _behavior = behavior;
            _key = key;
            _value = value;
            _use_tag = true;
        }

        public Widget Widget {
            get => _widget;
            set {
                if (_widget == value)
                    return;

                if (_widget is { }) {
                    RemoveAllPersistentEvents();
                    if (_use_tag)
                        _widget.BehaviorTags[_behavior.GetType()][_key] = null;

                    _behavior.Parent.Remove(_widget);
                }

                _widget = value;

                if (value == null)
                    return;

                AddAllPersistentEvents();

                if (_use_tag)
                    _behavior.SetTag(value, _key, _value);

                _behavior.Parent.Add(value);
            }
        }

        public bool FindIn(WidgetList widgets) {
            if (!_use_tag)
                throw new Exception("Cannot look for matching tags because tags were not given on construction.");

            var type = _behavior.GetType();
            foreach (var w in widgets) {
                if (w.BehaviorTags[type][_key] != _value)
                    continue;

                Widget = w; // TODO: property is complex enough to be a method
                return true;
            }

            return false;
        }

        public void Forget() =>
            Widget = null;

        void MaybeAddHandler(
            string nameof_event,
            Delegate handler
        ) =>
            MaybeDoHandler(nameof_event, e => e.AddEventHandler(_widget, handler));

        void MaybeRemoveHandler(
            string nameof_event,
            Delegate handler
        ) =>
            MaybeDoHandler(nameof_event, e => e.RemoveEventHandler(_widget, handler));

        void MaybeDoHandler(
            string nameof_event,
            Action<EventInfo> eventAction
        ) {
            if (_widget is null)
                return;

            var type = _widget.GetType();
            var maybe_event = type.GetEvent(nameof_event);
            if (!(maybe_event is { } @event))
                throw new InvalidOperationException($"Failed to find event of name '{nameof_event}' on type '{type.Name}'.");

            eventAction(@event);
        }

        public void AddPersistentEvent(string nameof_event, Action<object, EventArgs> action) {
            if (_persistent_events.TryGetValue(nameof_event, out var handler))
                handler += new EventHandler(action);
            else {
                handler = new EventHandler(action);
                _persistent_events.Add(nameof_event, handler);
            }

            MaybeAddHandler(nameof_event, handler);
        }

        public void AddPersistentEvent(string nameof_event, Action<object, RectangleFSetArgs> action) {
            if (_persistent_resize_events.TryGetValue(nameof_event, out var handler))
                handler += (s, e) => action(s, e);
            else {
                handler = (s, e) => action(s, e);
                _persistent_resize_events.Add(nameof_event, handler);
            }

            MaybeAddHandler(nameof_event, handler);
        }

        public void AddPersistentEvent(string nameof_event, Action<object, Point2SetArgs> action) {
            if (_persistent_point2set_events.TryGetValue(nameof_event, out var handler))
                handler += (s, e) => action(s, e);
            else {
                handler = (s, e) => action(s, e);
                _persistent_point2set_events.Add(nameof_event, handler);
            }

            MaybeAddHandler(nameof_event, handler);
        }

        // TODO: concat these collections somehow?
        void AddAllPersistentEvents() {
            foreach (var (name, action) in _persistent_events) MaybeAddHandler(name, action);
            foreach (var (name, action) in _persistent_resize_events) MaybeAddHandler(name, action);
            foreach (var (name, action) in _persistent_point2set_events) MaybeAddHandler(name, action);
        }

        void RemoveAllPersistentEvents() {
            foreach (var (name, action) in _persistent_events) MaybeRemoveHandler(name, action);
            foreach (var (name, action) in _persistent_resize_events) MaybeRemoveHandler(name, action);
            foreach (var (name, action) in _persistent_point2set_events) MaybeRemoveHandler(name, action);
        }
    }
}
