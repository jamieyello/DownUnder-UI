using DownUnder.UIEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DownUnder.UI.Widgets.WidgetCoding
{
    public class CodeConnectionEntry
    {
        public Widget Widget { get; private set; }
        EventInfo _event;
        MethodInfo _method;
        Delegate _delegate;
        WidgetCode _code;

        public bool Connected { get; private set; }

        public CodeConnectionEntry(WidgetCode code, Widget widget, EventInfo @event, MethodInfo method)
        {
            Widget = widget;
            _event = @event;
            _method = method;
            _code = code;
        }

        public void Connect()
        {
            _delegate = Delegate.CreateDelegate(_event.EventHandlerType, _code, _method);
            _event.GetAddMethod().Invoke(Widget, new object[] { _delegate });
            Connected = true;
        }

        public void Disconnect()
        {
            _event.GetRemoveMethod().Invoke(Widget, new object[] { _delegate });
            Connected = false;
        }
    }
}
