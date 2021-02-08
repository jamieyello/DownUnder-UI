using System;
using System.Reflection;

namespace DownUnder.UI.UI.Widgets.Behaviors.Functional
{
    /// <summary> Used to "connect" <see cref="Action"/>s to widget event handlers. Used to trigger actions from user input. </summary>
    public class TriggerAction : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        private EventInfo _event;
        private Delegate _delegate;

        /// <summary> The action that will be set off. </summary>
        public Action Action { get; set; }

        /// <summary> The name of the <see cref="Widget"/> <see cref="EventHandler"/> that will add the action to the widget. Use the nameof() C# expression (or your language's equivalent) to set this consistently. </summary>
        public string NameofEventHandler { get; set; }

        /// <summary> When set to true, the given action will be cloned. </summary>
        public bool CloneAction { get; set; } = true;

        public TriggerAction() { }
        public TriggerAction(string nameof_eventhandler, Action action)
        {
            NameofEventHandler = nameof_eventhandler;
            Action = action;
        }

        protected override void Initialize()
        {
            _event = Parent.GetType().GetEvent(NameofEventHandler);
            _delegate = Delegate.CreateDelegate(_event.EventHandlerType, this, nameof(InvokeAction));
        }

        protected override void ConnectEvents()
        {
            _event.AddEventHandler(Parent, _delegate);
        }

        protected override void DisconnectEvents()
        {
            _event.RemoveEventHandler(Parent, _delegate);
        }

        public override object Clone()
        {
            TriggerAction c = new TriggerAction();
            if (Action != null) c.Action = Action;
            c.NameofEventHandler = NameofEventHandler;
            c.CloneAction = CloneAction;
            return c;
        }

        public void InvokeAction(object sender, EventArgs args)
        {
            Action.Invoke();
        }
    }
}
