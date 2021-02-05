using MonoGame.Extended;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.DataTypes.InnerWidgetLocations
{
    public class CorneredLocation : InnerWidgetLocation
    {
        public DiagonalDirection2D Corner;

        public CorneredLocation()
        {
            Corner = DiagonalDirection2D.top_left;
        }
        public CorneredLocation(DiagonalDirection2D corner)
        {
            Corner = corner;
        }

        public override RectangleF GetLocation(Widget spawner, Widget widget)
        {
            return widget.Area.BorderingInside(spawner.Area, new DiagonalDirections2D(Corner));
        }

        public override object Clone()
        {
            CorneredLocation c = new CorneredLocation(Corner);
            return c;
        }
    }
}
