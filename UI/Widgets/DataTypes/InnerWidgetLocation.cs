using DownUnder.UI.Widgets.DataTypes.InnerWidgetLocations;
using MonoGame.Extended;
using System;

namespace DownUnder.UI.Widgets.DataTypes
{
    public abstract class InnerWidgetLocation : ICloneable
    {
        public abstract RectangleF GetLocation(Widget spawner, Widget widget);
        public abstract object Clone();

        public void SetLocation(Widget spawner, Widget widget)
        {
            widget.Area = GetLocation(spawner, widget);
        }

        public static CenteredLocation Centered => new CenteredLocation();
        public static CorneredLocation InsideTopRight => new CorneredLocation(Utilities.DiagonalDirection2D.top_right);
        public static CorneredLocation InsideTopLeft => new CorneredLocation(Utilities.DiagonalDirection2D.top_left);
        public static CorneredLocation InsideBottomLeft => new CorneredLocation(Utilities.DiagonalDirection2D.bottom_left);
        public static CorneredLocation InsideBottomRight => new CorneredLocation(Utilities.DiagonalDirection2D.bottom_right);
        public static BorderingSide OutsideTop => new BorderingSide(Utilities.Direction2D.up);
        public static BorderingSide OutsideBottom => new BorderingSide(Utilities.Direction2D.down);
        public static BorderingSide OutsideLeft => new BorderingSide(Utilities.Direction2D.left);
        public static BorderingSide OutsideRight => new BorderingSide(Utilities.Direction2D.right);
    }
}