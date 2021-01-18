using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets
{
    public class WidgetUpdateFlags
    {
        public bool _update_clicked_on;
        public bool _update_right_clicked_on;
        public bool _update_clicked_off;
        public bool _update_double_clicked;
        public bool _update_triple_clicked;
        public bool _update_added_to_focused;
        public bool _update_set_as_focused;
        public bool _update_hovered_over;
        public bool _update_drag;
        public bool _update_drop;

        public void Reset()
        {
            _update_clicked_on = false;
            _update_right_clicked_on = false;
            _update_double_clicked = false;
            _update_triple_clicked = false;
            _update_added_to_focused = false;
            _update_set_as_focused = false;
            _update_hovered_over = false;
            _update_drag = false;
            _update_drop = false;
            _update_clicked_off = false;
        }
    }
}
