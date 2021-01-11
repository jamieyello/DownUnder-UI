using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.WidgetCoding
{
    [DataContract(IsReference = true)]
    public abstract class WidgetCode
    {
        private static readonly EventInfo[] events = typeof(Widget).GetEvents();
        private MethodInfo[] methods;
        private List<ConnectionEntry> connections = new List<ConnectionEntry>();

        public WidgetCode()
        {
            SetNonSerialized();
        }

        [OnDeserialized]
        void SetNonSerialized(StreamingContext context) => SetNonSerialized();
        void SetNonSerialized()
        {
            methods = GetType().GetRuntimeMethods().ToArray();
            connections = new List<ConnectionEntry>();
        }

        public void Connect(Widget widget)
        {
            foreach (var match in GetMatches(widget))
            {
                ConnectionEntry connection = new ConnectionEntry(this, widget, match.Key, match.Value);
                connection.Connect();
                connections.Add(connection);
            }
        }

        public void Disconnect(Widget widget)
        {
            var list = new List<ConnectionEntry>(from c in connections where c.Widget == widget select c);
            foreach (var connection in list)
            {
                connection.Disconnect();
                connections.Remove(connection);
            }
        }

        Dictionary<EventInfo, MethodInfo> GetMatches(Widget widget)
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
