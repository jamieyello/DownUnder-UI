using MonoGame.Extended;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.DataTypes
{
    [DataContract]
    public abstract class OverlayWidgetLocation : ICloneable
    {
        public abstract RectangleF GetLocation(Widget spawner, Widget widget);
        public abstract object Clone();

        public void ApplyLocation(Widget spawner, Widget widget)
        {
            widget.Area = GetLocation(spawner, widget);
        }
    }
}
