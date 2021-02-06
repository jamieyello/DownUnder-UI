using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets
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
