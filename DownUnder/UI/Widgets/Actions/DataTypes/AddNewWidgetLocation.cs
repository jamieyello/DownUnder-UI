using DownUnder.Utilities;
using System;

namespace DownUnder.UI.Widgets.Actions.DataTypes
{
    public class AddNewWidgetLocation : ICloneable
    {
        public int ParentUp;
        public Direction2D ParentSide;

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
