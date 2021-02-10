using System;
using System.Reflection;

namespace DownUnder.UI.UI.Widgets.Behaviors.Functional {
    /// <summary> Used to "connect" <see cref="Action"/>s to widget event handlers. Used to trigger actions from user input. </summary>
    public sealed class TriggerAction : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.FUNCTION };

        EventInfo _event;
        Delegate _delegate;

        /// <summary> The action that will be set off. </summary>
        public Action Action { get; set; }

        /// <summary> The name of the <see cref="Widget"/> <see cref="EventHandler"/> that will add the action to the widget. Use the nameof() C# expression (or your language's equivalent) to set this consistently. </summary>
        public string NameofEventHandler { get; set; }

        /// <summary> When set to true, the given action will be cloned. </summary>
        public bool CloneAction { get; set; } = true;

        /// <summary> If will not trigger <see cref="Action"/> if <see cref="Widget.IsActive"/> is false. false by default. </summary>
        public bool RespectActivation { get; set; } = false;

        public TriggerAction() {
        }

        public TriggerAction(string nameof_eventhandler, Action action) {
            NameofEventHandler = nameof_eventhandler;
            Action = action;
        }

        protected override void Initialize() {
            var type = Parent.GetType();

            _event = type.GetEvent(NameofEventHandler);
            if (_event is null)
                throw new InvalidOperationException($"Failed to find event of type '{NameofEventHandler}' on type '{type.Name}'.");

            var maybe_event_type = _event.EventHandlerType;
            if (!(maybe_event_type is { } event_type))
                throw new InvalidOperationException($"Event '{_event.Name}' on type '{type.Name}' had no EventHandlerType.");

            _delegate = Delegate.CreateDelegate(event_type, this, nameof(InvokeAction));
        }

        protected override void ConnectEvents() =>
            _event.AddEventHandler(Parent, _delegate);

        protected override void DisconnectEvents() =>
            _event.RemoveEventHandler(Parent, _delegate);

        public override object Clone() {
            var c = new TriggerAction();
            if (Action is { })
                c.Action = Action;
            c.NameofEventHandler = NameofEventHandler;
            c.CloneAction = CloneAction;
            c.RespectActivation = RespectActivation;
            return c;
        }

        public void InvokeAction(object sender, EventArgs args) {
            if (Parent.IsActive || !RespectActivation) Action.Invoke();
        }
    }
}
