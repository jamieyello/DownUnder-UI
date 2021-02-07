using System;
using System.Runtime.Serialization;
using DownUnder.UI.UI.Widgets.DataTypes.InnerWidgetLocations;
using DownUnder.UI.Utilities.CommonNamespace;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.DataTypes
{
    [DataContract]
    public abstract class InnerWidgetLocation : ICloneable
    {
        public abstract RectangleF GetLocation(Widget spawner, Widget widget);
        public abstract object Clone();

        public void SetLocation(Widget spawner, Widget widget)
        {
            widget.Area = GetLocation(spawner, widget);
        }

        public static CenteredLocation Centered => new CenteredLocation();
        public static CorneredLocation InsideTopRight => new CorneredLocation(DiagonalDirection2D.top_right);
        public static CorneredLocation InsideTopLeft => new CorneredLocation(DiagonalDirection2D.top_left);
        public static CorneredLocation InsideBottomLeft => new CorneredLocation(DiagonalDirection2D.bottom_left);
        public static CorneredLocation InsideBottomRight => new CorneredLocation(DiagonalDirection2D.bottom_right);
        public static BorderingSide OutsideTop => new BorderingSide(Direction2D.up);
        public static BorderingSide OutsideBottom => new BorderingSide(Direction2D.down);
        public static BorderingSide OutsideLeft => new BorderingSide(Direction2D.left);
        public static BorderingSide OutsideRight => new BorderingSide(Direction2D.right);
        public static BorderingSide Outside(Direction2D direction) => new BorderingSide(direction);
    }
}