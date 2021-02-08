using System;
using DownUnder.UI.UI.Widgets.Behaviors;

namespace DownUnder.UI.UI.Widgets.CustomEventArgs
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
