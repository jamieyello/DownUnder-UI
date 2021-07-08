using System.Runtime.Serialization;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.DataTypes.OverlayWidgetLocations {
    [DataContract]
    public sealed class CoverParentOverlay : OverlayWidgetLocation {
        [DataMember] int ParentUp { get; set; }

        public CoverParentOverlay() {
        }

        public CoverParentOverlay(int parent_up) =>
            ParentUp = parent_up;

        public override RectangleF GetLocation(
            Widget spawner,
            Widget widget
        ) =>
            spawner.GetParentUp(ParentUp).AreaInWindow;

        public override object Clone() =>
            new CoverParentOverlay {
                ParentUp = ParentUp
            };
    }
}
