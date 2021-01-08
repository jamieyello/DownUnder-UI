using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets
{
    [DataContract(IsReference = true)]
    public abstract class WidgetCode
    {
        private static readonly EventInfo[] events = typeof(Widget).GetEvents();
        private MethodInfo[] methods;

        public WidgetCode()
        {
            methods = GetType().GetRuntimeMethods().ToArray();
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            methods = GetType().GetRuntimeMethods().ToArray();
        }

        public void ConnectMatches(Widget widget)
        {
            foreach (var match in GetMatches(widget))
            {
                var delegate_ = Delegate.CreateDelegate(match.Key.EventHandlerType, this, match.Value);
                match.Key.GetAddMethod().Invoke(widget, new object[] { delegate_ });
            }
        }

        public Dictionary<EventInfo, MethodInfo> GetMatches(Widget widget)
        {
            Dictionary<EventInfo, MethodInfo> matches = new Dictionary<EventInfo, MethodInfo>();

            for (int e = 0; e < events.Length; e++)
            {
                for (int m = 0; m < methods.Length; m++)
                {
                    if (widget.Name + "_" + events[e].Name == methods[m].Name) matches.Add(events[e], methods[m]);
                }
            }

            return matches;
        }
    }
}
