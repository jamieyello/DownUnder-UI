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
        public static CorneredLocation TopRight => new CorneredLocation(Utilities.DiagonalDirection2D.top_right);
        public static CorneredLocation TopLeft => new CorneredLocation(Utilities.DiagonalDirection2D.top_left);
        public static CorneredLocation BottomLeft => new CorneredLocation(Utilities.DiagonalDirection2D.bottom_left);
        public static CorneredLocation BottomRight => new CorneredLocation(Utilities.DiagonalDirection2D.bottom_right);
    }
}