using DownUnder.UI.Utilities.CommonNamespace;
using DownUnder.UI.Utilities.Extensions;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.DataTypes.InnerWidgetLocations
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
