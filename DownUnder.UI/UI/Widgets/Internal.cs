using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DownUnder.UI.Utilities.Serialization;

namespace DownUnder.UI.UI.Widgets {
    public sealed class Internal {
        const string _DUWDL_PATH = "WidgetIndexer.duwdl";

        private static readonly ReadOnlyDictionary<string, string> widget_xml_locations = new ReadOnlyDictionary<string, string>(XmlHelper.FromXmlFile<Dictionary<string, string>>(_DUWDL_PATH));

        public static bool TryGetWidgetXmlLocation(
            Type widget_code_type,
            out string value
        ) {
            var key = widget_code_type.Namespace + " " + widget_code_type.Name;
            return widget_xml_locations.TryGetValue(key, out value);
        }
    }
}
