using System;

namespace DownUnder.UI.UI.Widgets.CustomEventArgs {
    public sealed class PreUpdateArgs : EventArgs {
        public WidgetUpdateFlags Flags { get; }

        public PreUpdateArgs(WidgetUpdateFlags flags) =>
            Flags = flags;
    }
}