using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets
{
    public class WidgetArgs : EventArgs
    {
        public readonly Widget Widget;

        public WidgetArgs(Widget widget)
        {
            Widget = widget;
        }
    }
}
