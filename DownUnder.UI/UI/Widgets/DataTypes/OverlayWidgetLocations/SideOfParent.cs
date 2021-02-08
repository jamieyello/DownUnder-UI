using System.Runtime.Serialization;
using DownUnder.UI.Utilities.CommonNamespace;
using DownUnder.UI.Utilities.Extensions;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.DataTypes.OverlayWidgetLocations
{
    [DataContract] public class SideOfParent : OverlayWidgetLocation
    {
        [DataMember] public int ParentUp { get; set; } = 0;
        [DataMember] public Direction2D ParentSide = Direction2D.down;

        public SideOfParent() { }
        public SideOfParent(int parent_up, Direction2D parent_side)
        {
            ParentUp = parent_up;
            ParentSide = parent_side;
        }

        public override RectangleF GetLocation(Widget spawner, Widget widget)
        {
            return spawner.AreaInWindow.BorderingOutside(spawner.GetParentUp(ParentUp).AreaInWindow, ParentSide);
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
