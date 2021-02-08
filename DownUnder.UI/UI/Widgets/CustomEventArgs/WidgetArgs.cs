using System;

namespace DownUnder.UI.UI.Widgets.CustomEventArgs
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
