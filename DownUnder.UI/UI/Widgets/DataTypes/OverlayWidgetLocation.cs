using DownUnder.UI.Widgets.DataTypes.OverlayWidgetLocations;
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

        public static CoverParentOverlay CoverParent(int parent_up) => new CoverParentOverlay(parent_up);
        public static SideOfParent SideOfParent(int parent_up, Direction2D parent_side) => new SideOfParent(parent_up, parent_side);
    }
}
