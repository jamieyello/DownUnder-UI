using System;
using System.Reflection;

namespace DownUnder.UI {
    public static class EventInfoExtensions {
        public static MethodInfo GetAddMethodOrThrow(
            this EventInfo me
        ) {
            var maybe_add_method = me.GetAddMethod();
            if (!(maybe_add_method is { } add_method))
                throw new InvalidOperationException($"Failed to find Add method for event '{me.Name}'.");

            return add_method;
        }

        public static MethodInfo GetRemoveMethodOrThrow(
            this EventInfo me
        ) {
            var maybe_remove_method = me.GetRemoveMethod();
            if (!(maybe_remove_method is { } remove_method))
                throw new InvalidOperationException($"Failed to find Remove method for event '{me.Name}'.");

            return remove_method;
        }
    }
}