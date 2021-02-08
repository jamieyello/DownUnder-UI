using System.Runtime.Serialization;
using DownUnder.UI.Utilities.Extensions;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.DataTypes.OverlayWidgetLocations
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
