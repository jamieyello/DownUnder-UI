namespace DownUnder.UI.Widgets {
    public sealed class WidgetUpdateFlags {
        public bool clicked_on { get; set; }
        public bool right_clicked_on { get; set; }
        public bool clicked_off { get; set; }
        public bool double_clicked { get; set; }
        public bool triple_clicked { get; set; }
        public bool added_to_focused { get; set; }
        public bool set_as_focused { get; set; }
        public bool hovered_over { get; set; }
        public bool drag { get; set; }
        public bool drop { get; set; }
        public bool long_hover { get; set; }

        public void Reset() {
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
