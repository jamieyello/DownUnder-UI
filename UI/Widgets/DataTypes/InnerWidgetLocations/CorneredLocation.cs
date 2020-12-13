using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets.DataTypes.InnerWidgetLocations
{
    public class CorneredLocation : InnerWidgetLocation
    {
        DiagonalDirection2D Corner;

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
            throw new NotImplementedException();
        }
    }
}
