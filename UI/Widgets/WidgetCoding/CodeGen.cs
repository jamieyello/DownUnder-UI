using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.WidgetCoding
{
    static class CodeGen
    {
    }
}

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
