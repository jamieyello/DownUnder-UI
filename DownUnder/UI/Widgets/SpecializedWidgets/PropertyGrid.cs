using DownUnder.UI.Widgets.BaseWidgets;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.SpecializedWidgets {
    public class PropertyGrid : Grid {
        bool _editing_enabled_backing = false;

        /// <summary> Returns true if the there are any properties in the given object to view. </summary>
        public static bool IsCompatible(object obj) => obj.GetType().GetProperties().Length != 0;

        [DataMember] public bool EditingEnabled {
            get => _editing_enabled_backing;
            set {
                var y = Dimensions.Y;
                for (int i = 0; i < y; i++) {
                    ObjectLabel object_label = (ObjectLabel)GetCell(1, i);
                    object_label.EditingEnabled = value;
                } 
                _editing_enabled_backing = value;
            }
        }
        
        public PropertyGrid(IParent parent, object obj) : base(parent, 2, obj.GetType().GetProperties().Length)
        {
            var properties = obj.GetType().GetProperties();
            if (properties.Length == 0) throw new Exception("No properties in object.");

            Parallel.ForEach(properties, (property, state, index) => {
                int i = (int)index;
                SetCell(0, i, new Label(this, properties[index].Name) { DrawOutline = true, OutlineSides = Directions2D.DR, MinimumHeight = 20 });

                if (property.GetIndexParameters().Length != 0)
                    SetCell(1, i, new Label(this, "Collection...") { DrawOutline = true, OutlineSides = Directions2D.DR, MinimumHeight = 20 });
                else if (properties[index].GetValue(obj) == null) 
                    SetCell(1, i, new Label(this, "null") { DrawOutline = true, OutlineSides = Directions2D.DR, MinimumHeight = 20 });
                else
                {
                    var label = new ObjectLabel(this, properties[index].GetValue(obj)) { DrawOutline = true, OutlineSides = Directions2D.DR, MinimumHeight = 20 };
                    SetCell(1, i, label);
                }
            });
            
        }
    }
}
