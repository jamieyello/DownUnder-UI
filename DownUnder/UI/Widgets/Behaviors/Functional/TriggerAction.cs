using DownUnder.UI.Widgets.Actions;
using System;
using System.Reflection;

namespace DownUnder.UI.Widgets.Behaviors.Functional
{
    public class TriggerAction : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        private EventInfo _event;
        private Delegate _delegate;

        public WidgetAction Action { get; set; }
        public string NameofEventHandler { get; set; }
        public bool CloneAction { get; set; } = true;

        public TriggerAction() { }
        public TriggerAction(string nameof_eventhandler, WidgetAction action)
        {
            NameofEventHandler = nameof_eventhandler;
            Action = action;
        }

        protected override void Initialize()
        {
            _event = Parent.GetType().GetEvent(NameofEventHandler);
            _delegate = Delegate.CreateDelegate(_event.EventHandlerType, this, nameof(AddAction));
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
            if (Action != null) c.Action = (WidgetAction)Action.InitialClone();
            c.NameofEventHandler = NameofEventHandler;
            c.CloneAction = CloneAction;
            return c;
        }

        public void AddAction(object sender, EventArgs args)
        {
            if (CloneAction) Parent.Actions.Add((WidgetAction)Action.InitialClone());
            else Parent.Actions.Add(Action);
        }
    }
}
