using DownUnder.UI.UI.Widgets.DataTypes;

namespace DownUnder.UI {
    public static class BorderSizeExtensions {
        public static BorderSize SetTop(this BorderSize me, float value) => new BorderSize(value, me.Bottom, me.Left, me.Right);
        public static BorderSize SetBottom(this BorderSize me, float value) => new BorderSize(me.Top, value, me.Left, me.Right);
        public static BorderSize SetLeft(this BorderSize me, float value) => new BorderSize(me.Top, me.Bottom, value, me.Right);
        public static BorderSize SetRight(this BorderSize me, float value) => new BorderSize(me.Top, me.Bottom, me.Left, value);
    }
}