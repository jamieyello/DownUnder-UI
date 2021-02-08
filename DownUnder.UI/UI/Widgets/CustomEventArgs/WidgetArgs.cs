using System;

namespace DownUnder.UI.UI.Widgets.CustomEventArgs {
    public sealed class WidgetArgs : EventArgs {
        public Widget Widget { get; }

        public WidgetArgs(Widget widget) =>
            Widget = widget;
    }
}