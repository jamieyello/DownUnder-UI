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
        // TT file below
        public const string DefaultTTCode = "<#@ template debug=\"false\" hostspecific=\"false\" language=\"C#\" #>\n<#@ output extension=\".cs\" #>\n<# var widget_names = new string [] { /*widget_names*/}; #>\n<# var class_name = \"MyWidgetCode\"; #>\n<# string used_namespace = (string)System.Runtime.Remoting.Messaging.CallContext.LogicalGetData(\"NamespaceHint\"); #>\n// This is generated code;\nusing DownUnder.UI.Widgets;\nusing DownUnder.UI.Widgets.WidgetCoding;\n\nnamespace <#= used_namespace.Replace(' ', '_') #>\n{\n	partial class MyWidgetCode : WidgetCode\n	{\n<# foreach (string widget_name in widget_names) { #>\n		/// <summary> Auto generated accessor for <#= widget_name #>. </summary>\n		public Widget<#= widget_name #> => Base[\"<#= widget_name #>\"];\n\n<# } #>\n	}\n	}\n";

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

// TT File:

//<#@ template debug="false" hostspecific="false" language="C#" #>
//<#@ output extension=".cs" #>
//<# var widget_names = new string [] { /*widget_names*/}; #>
//<# var class_name = "MyWidgetCode"; #>
//<# string used_namespace = (string)System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("NamespaceHint"); #>
//// This is generated code;
//using DownUnder.UI.Widgets;
//using DownUnder.UI.Widgets.WidgetCoding;

//namespace <#= used_namespace.Replace(' ', '_') #>
//{
//	partial class MyWidgetCode : WidgetCode
//{
//<# foreach (string widget_name in widget_names) { #>
//		/// <summary> Auto generated accessor for <#= widget_name #>. </summary>
//		public Widget<#= widget_name #> => Base["<#= widget_name #>"];

//<# } #>
//	}
//}
