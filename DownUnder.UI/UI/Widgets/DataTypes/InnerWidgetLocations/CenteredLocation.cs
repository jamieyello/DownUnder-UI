using DownUnder.UI.Utilities.Extensions;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.DataTypes.InnerWidgetLocations
{
    public class CenteredLocation : InnerWidgetLocation
    {
        public override RectangleF GetLocation(Widget spawner, Widget widget)
        {
            return widget.Area.WithCenter(spawner.Area.SizeOnly().Center);
        }

        public override object Clone()
        {
            return new CenteredLocation();
        }
    }
}
