using DownUnder.UI.Widgets.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets
{
    public class WidgetBehaviorArgs : EventArgs
    {
        public readonly WidgetBehavior Behavior;

        public WidgetBehaviorArgs(WidgetBehavior behavior)
        {
            Behavior = behavior;
        }
    }
}
