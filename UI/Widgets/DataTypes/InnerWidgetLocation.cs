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
    }
}