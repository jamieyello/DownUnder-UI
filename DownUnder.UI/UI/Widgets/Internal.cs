using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DownUnder.UI.Utilities.Serialization;

namespace DownUnder.UI.UI.Widgets {
    public sealed class Internal {
        const string DUWDL_PATH = "WidgetIndexer.duwdl";

        public static readonly ReadOnlyDictionary<string, string> WidgetXMLLocations = new ReadOnlyDictionary<string, string>(XmlHelper.FromXmlFile<Dictionary<string, string>>(DUWDL_PATH));

        public static bool TryGetWidgetXMLLocation(
            Type widget_code_type,
            out string value
        ) {
            var key = widget_code_type.Namespace + " " + widget_code_type.Name;
            return WidgetXMLLocations.TryGetValue(key, out value);
        }
    }
}
