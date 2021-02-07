using System;

namespace DownUnder.UI.UI.Widgets.CustomEventArgs
{
    public class PreUpdateArgs : EventArgs
    {
        public readonly WidgetUpdateFlags Flags;

        public PreUpdateArgs(WidgetUpdateFlags flags)
        {
            Flags = flags;
        }
    }
}
