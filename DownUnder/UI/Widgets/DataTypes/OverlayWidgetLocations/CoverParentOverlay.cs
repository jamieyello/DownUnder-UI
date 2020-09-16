using MonoGame.Extended;

namespace DownUnder.UI.Widgets.DataTypes.OverlayWidgetLocations
{
    class CoverParentOverlay : OverlayWidgetLocation
    {
        int ParentUp { get; set; } = 0;

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
