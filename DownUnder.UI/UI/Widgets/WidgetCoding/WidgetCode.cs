using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using DownUnder.UI.Utilities.CommonNamespace;

namespace DownUnder.UI.UI.Widgets.WidgetCoding {
    [DataContract(IsReference = true)]
    public abstract class WidgetCode : IWidget {
        [DataMember] public readonly Widget Base;
        static readonly EventInfo[] events = typeof(Widget).GetEvents();
        MethodInfo[] methods;
        List<CodeConnectionEntry> connections = new List<CodeConnectionEntry>();

        Widget IWidget.Widget => Base;

        protected WidgetCode() {
            if (!Internal.TryGetWidgetXMLLocation(GetType(), out var xml_path))
                throw new Exception($"No matching xml file was found for this {nameof(WidgetCode)}. Running the Widget Editor tool on the .duwd file can update references.");

            Base = Widget.LoadFromXML(xml_path);
            SetNonSerialized();
            Connect(Base);
        }

        [OnDeserialized]
        void SetNonSerialized(StreamingContext context) =>
            SetNonSerialized();

        void SetNonSerialized() {
            methods = GetType().GetRuntimeMethods().ToArray();
            connections = new List<CodeConnectionEntry>();
        }

        public void Connect(Widget widget) {
            foreach (var match in GetMatches(widget)) {
                var connection = new CodeConnectionEntry(this, widget, match.Key, match.Value);
                connection.Connect();
                connections.Add(connection);
            }
        }

        public void Disconnect(Widget widget) {
            var list = new List<CodeConnectionEntry>(from c in connections where c.Widget == widget select c);
            foreach (var connection in list) {
                connection.Disconnect();
                connections.Remove(connection);
            }
        }

        Dictionary<EventInfo, MethodInfo> GetMatches(Widget widget) {
            var matches = new Dictionary<EventInfo, MethodInfo>();

            foreach (var e in events)
            foreach (var m in methods) {
                var name = $"{widget.Name}_{e.Name}";
                if (name != m.Name)
                    continue;

                matches.Add(e, m);
            }

            return matches;
        }
    }
}
