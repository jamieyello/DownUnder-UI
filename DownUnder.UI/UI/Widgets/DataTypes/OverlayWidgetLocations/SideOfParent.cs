using System.Runtime.Serialization;
using MonoGame.Extended;
using DownUnder.UI.Utilities.CommonNamespace;

namespace DownUnder.UI.UI.Widgets.DataTypes.OverlayWidgetLocations {
    [DataContract]
    public sealed class SideOfParent : OverlayWidgetLocation {
        [DataMember] public int ParentUp { get; set; }
        [DataMember] public Direction2D ParentSide = Direction2D.down;

        public SideOfParent() {
        }

        public SideOfParent(int parent_up, Direction2D parent_side) {
            ParentUp = parent_up;
            ParentSide = parent_side;
        }

        public override RectangleF GetLocation(Widget spawner, Widget widget) =>
            spawner.AreaInWindow.BorderingOutside(spawner.GetParentUp(ParentUp).AreaInWindow, ParentSide);

        public override object Clone() =>
            new SideOfParent {
                ParentUp = ParentUp,
                ParentSide = ParentSide
            };
    }
}