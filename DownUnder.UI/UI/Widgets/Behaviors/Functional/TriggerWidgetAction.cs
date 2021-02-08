using System;
using System.Reflection;
using DownUnder.UI.UI.Widgets.Actions;

namespace DownUnder.UI.UI.Widgets.Behaviors.Functional {
    /// <summary> Used to "connect" <see cref="WidgetAction"/>s to widget event handlers. Used to trigger actions from user input. </summary>
    public sealed class TriggerWidgetAction : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.FUNCTION };

        EventInfo _event;
        Delegate _delegate;

        /// <summary> The action that will be set off. </summary>
        public WidgetAction Action { get; set; }

        /// <summary> The name of the <see cref="Widget"/> <see cref="EventHandler"/> that will add the action to the widget. Use the nameof() C# expression (or your language's equivalent) to set this consistently. </summary>
        public string NameofEventHandler { get; set; }

        /// <summary> When set to true, the given action will be cloned. </summary>
        public bool CloneAction { get; set; } = true;

        public TriggerWidgetAction() {
        }

        public TriggerWidgetAction(string nameof_eventhandler, WidgetAction action) {
            NameofEventHandler = nameof_eventhandler;
            Action = action;
        }

        protected override void Initialize() {
            var type = Parent.GetType();

            _event = type.GetEvent(NameofEventHandler);
            if (_event is null)
                throw new InvalidOperationException($"Event of name '{NameofEventHandler}' on type '{type.Name}' was not found.");

            var maybe_event_type = _event.EventHandlerType;
            if (!(maybe_event_type is { } event_type))
                throw new InvalidOperationException($"Event '{_event.Name}' on type '{type.Name}' had no EventHandlerType.");

            _delegate = Delegate.CreateDelegate(event_type, this, nameof(AddAction));
        }

        protected override void ConnectEvents() =>
            _event.AddEventHandler(Parent, _delegate);

        protected override void DisconnectEvents() =>
            _event.RemoveEventHandler(Parent, _delegate);

        public override object Clone() {
            var c = new TriggerWidgetAction();

            if (Action is { })
                c.Action = (WidgetAction)Action.InitialClone();

            c.NameofEventHandler = NameofEventHandler;
            c.CloneAction = CloneAction;
            return c;
        }

        public void AddAction(object sender, EventArgs args) {
            if (CloneAction)
                Parent.Actions.Add((WidgetAction)Action.InitialClone());
            else
                Parent.Actions.Add(Action);
        }
    }
}