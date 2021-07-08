using DownUnder.UI.Widgets;
using System;

namespace DownUnder.UI {
    public sealed class PreUpdateArgs : EventArgs {
        public WidgetUpdateFlags Flags { get; }

        public PreUpdateArgs(WidgetUpdateFlags flags) =>
            Flags = flags;
    }
}