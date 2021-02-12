using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.DataTypes.InnerWidgetLocations {
    public sealed class CenteredLocation : InnerWidgetLocation {
        public override RectangleF GetLocation(Widget spawner, Widget widget) =>
            widget.Area.WithCenter(spawner.Area.SizeOnly().Center);

        public override object Clone() =>
            new CenteredLocation();
    }
}
