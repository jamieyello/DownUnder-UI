using DownUnder.Utilities;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.DataTypes.InnerWidgetLocations
{
    public class BorderingSide : InnerWidgetLocation
    {
        public Direction2D Side { get; set; }

        public BorderingSide(Direction2D side)
        {
            Side = side;
        }

        public override object Clone()
        {
            return new BorderingSide(Side);
        }

        public override RectangleF GetLocation(Widget spawner, Widget widget)
        {
            return widget.Area.BorderingOutside(spawner.Area, Side);
        }
    }
}
