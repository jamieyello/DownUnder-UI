namespace DownUnder.UI.UI.Widgets
{
    public class WidgetUpdateFlags
    {
        public bool clicked_on;
        public bool right_clicked_on;
        public bool clicked_off;
        public bool double_clicked;
        public bool triple_clicked;
        public bool added_to_focused;
        public bool set_as_focused;
        public bool hovered_over;
        public bool drag;
        public bool drop;
        public bool long_hover;

        public void Reset()
        {
            clicked_on = false;
            right_clicked_on = false;
            double_clicked = false;
            triple_clicked = false;
            added_to_focused = false;
            set_as_focused = false;
            hovered_over = false;
            drag = false;
            drop = false;
            clicked_off = false;
            long_hover = false;
        }
    }
}
