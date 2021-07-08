using DownUnder.UI.Widgets;
using System;

namespace DownUnder.UI {
    public sealed class WidgetArgs : EventArgs {
        public Widget Widget { get; }

        public WidgetArgs(Widget widget) =>
            Widget = widget;
    }
}