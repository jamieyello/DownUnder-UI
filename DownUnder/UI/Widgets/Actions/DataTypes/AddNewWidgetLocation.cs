using DownUnder.Utilities;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Actions.DataTypes
{
    [DataContract] public class AddNewWidgetLocation : ICloneable
    {
        [DataMember] public int ParentUp { get; set; } = 0;
        [DataMember] public Direction2D ParentSide = Direction2D.down;

        public void ApplyLocation(Widget spawner, Widget widget)
        {
            Widget target_border = spawner;
            for (int i = 0; i < ParentUp; i++)
            {
                target_border = target_border.ParentWidget;
            }
            widget.Area = spawner.AreaInWindow.Bordering(target_border.AreaInWindow, ParentSide);
        }

        public object Clone()
        {
            AddNewWidgetLocation c = new AddNewWidgetLocation();
            c.ParentUp = ParentUp;
            c.ParentSide = ParentSide;
            return c;
        }
    }
}
