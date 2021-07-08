using System;
using System.Reflection;

namespace DownUnder.UI.Widgets.WidgetCoding {
    public sealed class CodeConnectionEntry {
        readonly EventInfo _event;
        readonly MethodInfo _method;
        Delegate _delegate;
        readonly WidgetCode _code;

        public Widget Widget { get; }

        public bool Connected { get; private set; }

        public CodeConnectionEntry(
            WidgetCode code,
            Widget widget,
            EventInfo @event,
            MethodInfo method
        ) {
            Widget = widget;
            _event = @event;
            _method = method;
            _code = code;
        }

        public void Connect() {
            if (!(_event.EventHandlerType is { } handler_type))
                throw new InvalidOperationException($"Event '{_event.Name}' had no EventHandlerType.");

            _delegate = Delegate.CreateDelegate(handler_type, _code, _method);
            _event.GetAddMethodOrThrow().Invoke(Widget, new object[] { _delegate });
            Connected = true;
        }

        public void Disconnect() {
            _event.GetRemoveMethodOrThrow().Invoke(Widget, new object[] { _delegate });
            Connected = false;
        }
    }
}
