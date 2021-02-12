using System;

namespace DownUnder.UI {
    public static class TypeExtensions {
        public static Type GetEventHandlerTypeOrThrow(
            this Type me,
            string event_name
        ) {
            var maybe_event = me.GetEvent(event_name);
            if (!(maybe_event is { } @event))
                throw new InvalidOperationException($"Failed to find event of name '{event_name}' on type '{me.Name}'.");

            var maybe_handler_type = @event.EventHandlerType;
            if (!(maybe_handler_type is { } handler_type))
                throw new InvalidOperationException($"Event '{@event.Name}' on type '{me.Name}' had no EventHandlerType.");

            return handler_type;
        }
    }
}
