using System;
using System.Runtime.Serialization;
using MonoGame.Extended;
using DownUnder.UI.UI.Widgets.DataTypes.InnerWidgetLocations;
using DownUnder.UI.Utilities.CommonNamespace;
using static DownUnder.UI.Utilities.CommonNamespace.Direction2D;
using static DownUnder.UI.Utilities.CommonNamespace.DiagonalDirection2D;

namespace DownUnder.UI.UI.Widgets.DataTypes {
    [DataContract]
    public abstract class InnerWidgetLocation : ICloneable {
        public abstract RectangleF GetLocation(Widget spawner, Widget widget);

        public abstract object Clone();

        public void SetLocation(Widget spawner, Widget widget) =>
            widget.Area = GetLocation(spawner, widget);

        public static CenteredLocation Centered => new CenteredLocation();
        public static CorneredLocation InsideTopRight => new CorneredLocation(top_right);
        public static CorneredLocation InsideTopLeft => new CorneredLocation(top_left);
        public static CorneredLocation InsideBottomLeft => new CorneredLocation(bottom_left);
        public static CorneredLocation InsideBottomRight => new CorneredLocation(bottom_right);
        public static BorderingSide OutsideTop => new BorderingSide(up);
        public static BorderingSide OutsideBottom => new BorderingSide(down);
        public static BorderingSide OutsideLeft => new BorderingSide(left);
        public static BorderingSide OutsideRight => new BorderingSide(right);
        public static BorderingSide Outside(Direction2D direction) => new BorderingSide(direction);
    }
}