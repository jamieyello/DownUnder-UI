using System;
using DownUnder.UI.Widgets.Behaviors;

namespace DownUnder.UI {
    public sealed class WidgetBehaviorArgs : EventArgs {
        public WidgetBehavior Behavior { get; }

        public WidgetBehaviorArgs(WidgetBehavior behavior) =>
            Behavior = behavior;
    }
}