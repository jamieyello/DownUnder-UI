using MonoGame.Extended;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.DataTypes.OverlayWidgetLocations
{
    [DataContract]
    public class CoverParentOverlay : OverlayWidgetLocation
    {
        [DataMember] int ParentUp { get; set; } = 0;

        public CoverParentOverlay() { }
        public CoverParentOverlay(int parent_up)
        {
            ParentUp = parent_up;
        }

        public override RectangleF GetLocation(Widget spawner, Widget widget)
        {
            return spawner.GetParentUp(ParentUp).AreaInWindow;
        }

        public override object Clone()
        {
            CoverParentOverlay c = new CoverParentOverlay();
            c.ParentUp = ParentUp;
            return c;
        }
    }
}
