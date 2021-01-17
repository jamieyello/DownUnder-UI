using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DownUnder.UI.Widgets.WidgetCoding
{
    [DataContract(IsReference = true)]
    public abstract class WidgetCode
    {
        public const string DefaultTTCode = "<#@ template debug=\"false\" hostspecific=\"false\" language=\"C#\" #><#@ output extension=\".cs\" #><# var widget_names = new string [] { /*widget_names*/}; #><# var class_name = \"MyWidgetCode\"; #><# string used_namespace = \"SafeNamespaceNameDUUI\"; #>// This is generated code;\nusing DownUnder.UI.Widgets;\nusing DownUnder.UI.Widgets.WidgetCoding;\n\nnamespace <#= used_namespace.Replace(' ', '_') #>\n{\n	partial class MyWidgetCode : WidgetCode\n	{\n<# foreach (string widget_name in widget_names) { #>		/// <summary> Auto generated accessor for <#= widget_name #>. </summary>\n		public Widget <#= widget_name #> => Base[\"<#= widget_name #>\"];\n\n<# } #>	}\n}\n";
        
        public static string GetTTCode(Widget base_widget, Type widget_code_type)
        {
            if (!typeof(WidgetCode).IsAssignableFrom(widget_code_type)) throw new Exception($"{nameof(WidgetCode)} is not assignable from passed type.");
            if (widget_code_type.Namespace == null) throw new Exception($"{nameof(WidgetCode)} does not have a usable namespace, cannot make TT code.");
            return GetTTCode(base_widget, widget_code_type.Namespace, widget_code_type.Name);
        }
        public static string GetTTCode(Widget base_widget, string code_namespace, string class_name)
        {
            StringBuilder result = new StringBuilder(DefaultTTCode);

            result.Replace("MyWidgetCode", class_name);
            result.Replace("SafeNamespaceNameDUUI", code_namespace);

            StringBuilder w_properties = new StringBuilder();
            foreach (Widget widget in base_widget.AllContainedWidgets)
            {
                if (widget == base_widget) continue;
                if (w_properties.Length == 0) w_properties.Append('"' + widget.Name + '"');
                else w_properties.Append(", \"" + widget.Name + '"');
            }
            result.Replace("/*widget_names*/", w_properties.ToString());

            return result.ToString();
        }

        [DataMember] public Widget Base { get; private set; }
        private static readonly EventInfo[] events = typeof(Widget).GetEvents();
        private MethodInfo[] methods;
        private List<CodeConnectionEntry> connections = new List<CodeConnectionEntry>();

        public WidgetCode(Widget @base)
        {
            Base = @base;
            SetNonSerialized();
        }

        [OnDeserialized]
        void SetNonSerialized(StreamingContext context) => SetNonSerialized();
        void SetNonSerialized()
        {
            methods = GetType().GetRuntimeMethods().ToArray();
            connections = new List<CodeConnectionEntry>();
        }

        public void Connect(Widget widget)
        {
            foreach (var match in GetMatches(widget))
            {
                CodeConnectionEntry connection = new CodeConnectionEntry(this, widget, match.Key, match.Value);
                connection.Connect();
                connections.Add(connection);
            }
        }

        public void Disconnect(Widget widget)
        {
            var list = new List<CodeConnectionEntry>(from c in connections where c.Widget == widget select c);
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