using MonoGame.Extended;
using DownUnder.UI.Utilities.CommonNamespace;

namespace DownUnder.UI.UI.Widgets.DataTypes.InnerWidgetLocations {
    public sealed class CorneredLocation : InnerWidgetLocation {
        public DiagonalDirection2D Corner { get; }

        public CorneredLocation() =>
            Corner = DiagonalDirection2D.top_left;

        public CorneredLocation(DiagonalDirection2D corner) =>
            Corner = corner;

        public override RectangleF GetLocation(
            Widget spawner,
            Widget widget
        ) =>
            widget.Area.BorderingInside(spawner.Area, new DiagonalDirections2D(Corner));

        public override object Clone() =>
            new CorneredLocation(Corner);
    }
}
