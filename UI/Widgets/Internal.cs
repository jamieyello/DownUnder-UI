using DownUnder.Content.Utilities.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DownUnder.UI.Widgets
{
    public class Internal
    {
        private const string DUWDL_PATH = "WidgetIndexer.duwdl";

        public readonly static ReadOnlyDictionary<string, string> WidgetXMLLocations = new ReadOnlyDictionary<string, string>(XmlHelper.FromXmlFile<Dictionary<string, string>>(DUWDL_PATH));
        
        public static bool TryGetWidgetXMLLocation(Type widget_code_type, out string value)
        {
            string key = widget_code_type.Namespace + " " + widget_code_type.Name;
            return WidgetXMLLocations.TryGetValue(key, out value);
        }
    }
}
