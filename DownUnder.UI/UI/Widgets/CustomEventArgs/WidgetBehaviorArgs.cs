using System;
using DownUnder.UI.UI.Widgets.Behaviors;

namespace DownUnder.UI.UI.Widgets.CustomEventArgs {
    public sealed class WidgetBehaviorArgs : EventArgs {
        public WidgetBehavior Behavior { get; }

        public WidgetBehaviorArgs(WidgetBehavior behavior) =>
            Behavior = behavior;
    }
}