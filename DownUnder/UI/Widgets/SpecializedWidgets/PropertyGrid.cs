using DownUnder.UI.Widgets.Interfaces;
using DownUnder.Utility;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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

        public PropertyGrid(IWidgetParent parent, SpriteFont sprite_font, object obj) 
            : base(parent, 2, obj.GetType().GetProperties().Length)
        {
            
            var properties = obj.GetType().GetProperties();
            if (properties.Length == 0) { throw new Exception("No properties in object."); }

            for (int i = 0; i < properties.Length; i++)
            {
                SetCell(0, i, new Label(
                    parent,
                    sprite_font,
                    properties[i].Name)
                {
                    DrawOutline = true,
                    OutlineSides = Directions2D.DownRight,
                    MinimumHeight = 20 // test
                });

                if (properties[i].GetValue(obj) == null)
                {
                    SetCell(1, i, new Label(
                        parent,
                        sprite_font, "null"
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
                    SetCell(1, i, new ObjectLabel(
                        parent,
                        sprite_font,
                        properties[i].GetValue(obj)
                        )
                    {
                        DrawOutline = true,
                        OutlineSides = Directions2D.DownRight,
                        MinimumHeight = 20
                    }
                    );
                }
            }
        }

        #endregion
    }
}
