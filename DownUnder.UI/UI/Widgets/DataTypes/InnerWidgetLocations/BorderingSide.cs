using MonoGame.Extended;
using DownUnder.UI.Utilities.CommonNamespace;

namespace DownUnder.UI.UI.Widgets.DataTypes.InnerWidgetLocations {
    public sealed class BorderingSide : InnerWidgetLocation {
        public Direction2D Side { get; }

        public BorderingSide(Direction2D side) =>
            Side = side;

        public override object Clone() =>
            new BorderingSide(Side);

        public override RectangleF GetLocation(Widget spawner, Widget widget) =>
            widget.Area.BorderingOutside(spawner.Area, Side);
    }
}
