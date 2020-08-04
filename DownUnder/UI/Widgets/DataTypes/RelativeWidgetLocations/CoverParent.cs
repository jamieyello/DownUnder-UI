using MonoGame.Extended;

namespace DownUnder.UI.Widgets.DataTypes.RelativeWidgetLocations
{
    class CoverParent : RelativeWidgetLocation
    {
        int ParentUp { get; set; } = 0;

        public CoverParent() { }
        public CoverParent(int parent_up)
        {
            ParentUp = parent_up;
        }

        public override RectangleF GetLocation(Widget spawner, Widget widget)
        {
            return spawner.GetParentUp(ParentUp).AreaInWindow;
        }

        public override object Clone()
        {
            CoverParent c = new CoverParent();
            c.ParentUp = ParentUp;
            return c;
        }
    }
}
