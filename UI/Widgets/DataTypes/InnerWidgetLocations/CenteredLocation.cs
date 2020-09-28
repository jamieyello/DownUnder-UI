using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.DataTypes.InnerWidgetLocations
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
