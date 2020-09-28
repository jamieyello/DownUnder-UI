using DownUnder.Utilities;
using MonoGame.Extended;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.DataTypes.OverlayWidgetLocations
{
    [DataContract] public class SideOfParent : OverlayWidgetLocation
    {
        [DataMember] public int ParentUp { get; set; } = 0;
        [DataMember] public Direction2D ParentSide = Direction2D.down;

        public override RectangleF GetLocation(Widget spawner, Widget widget)
        {
            return spawner.AreaInWindow.Bordering(spawner.GetParentUp(ParentUp).AreaInWindow, ParentSide);
        }

        public override object Clone()
        {
            SideOfParent c = new SideOfParent();
            c.ParentUp = ParentUp;
            c.ParentSide = ParentSide;
            return c;
        }
    }
}
