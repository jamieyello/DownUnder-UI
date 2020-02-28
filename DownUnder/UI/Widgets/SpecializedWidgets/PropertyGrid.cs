using DownUnder.UI.Widgets.BaseWidgets;
using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utility;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.SpecializedWidgets
{
    public class PropertyGrid : Grid
    {
        #region Fields

        bool _editing_enabled_backing = false;

        #endregion

        #region Public Properties

        [DataMember] public bool EditingEnabled
        {
            get => _editing_enabled_backing;
            set
            {
                var y = Dimensions.Y;
                for (int i = 0; i < y; i++)
                {
                    ObjectLabel object_label = (ObjectLabel)GetCell(1, i);
                    object_label.EditingEnabled = value;
                } 
                _editing_enabled_backing = value;
            }
        }
        
        #endregion

        #region Constructors

        public PropertyGrid(IParent parent, object obj) 
            : base(parent, 2, obj.GetType().GetProperties().Length)
        {
            bool previous_disable_update_area = _disable_update_area;
            _disable_update_area = true;
            var properties = obj.GetType().GetProperties();
            if (properties.Length == 0) { throw new Exception("No properties in object."); }
            
            for (int i = 0; i < properties.Length; i++)
            {
                SetCell(0, i, new Label(
                    this,
                    parent.SpriteFont,
                    properties[i].Name)
                {
                    DrawOutline = true,
                    OutlineSides = Directions2D.DownRight,
                    MinimumHeight = 20 // test
                });

                if (properties[i].GetIndexParameters().Length != 0)
                {
                    SetCell(1, i, new Label(
                        this,
                        parent.SpriteFont, "*Collection*"
                        )
                    {
                        DrawOutline = true,
                        OutlineSides = Directions2D.DownRight,
                        MinimumHeight = 20
                    }
);
                }
                else if (properties[i].GetValue(obj) == null)
                {
                    SetCell(1, i, new Label(
                        this,
                        parent.SpriteFont, "null"
                        )
                    {
                        DrawOutline = true,
                        OutlineSides = Directions2D.DownRight,
                        MinimumHeight = 20
                    }
                    );
                }
                else
                {
                    var label = new ObjectLabel(
                        this,
                        parent.SpriteFont,
                        properties[i].GetValue(obj)
                        )
                    {
                        DrawOutline = true,
                        OutlineSides = Directions2D.DownRight,
                        MinimumHeight = 20
                    };
                    SetCell(1, i, label);
                }
            }
            _disable_update_area = false;
        }

        #endregion

        #region Public Methods

        /// <summary> Returns true if the there are any properties in the given object to view. </summary>
        public static bool IsCompatible(object obj)
        {
            if (obj.GetType().GetProperties().Length == 0) return false;
            return true;
        }

        #endregion
    }
}
