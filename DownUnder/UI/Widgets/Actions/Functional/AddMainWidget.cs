using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Actions.Functional
{
    [DataContract] class AddMainWidget : WidgetAction
    {
        [DataMember] public Widget Widget { get; set; }
        [DataMember] public bool CloneWidgetOnAdd { get; set; } = true;
        [DataMember] public bool CloneWidgetInClone { get; set; } = true;

        public AddMainWidget() { }
        public AddMainWidget(Widget widget)
        {
            Widget = widget;
        }

        protected override void Initialize() 
        {
            if (Parent.ParentWindow != null) AddWidget(this, EventArgs.Empty);
            else Parent.OnParentWindowSet += AddWidget;
        }

        protected override void ConnectToParent() { }
        protected override void DisconnectFromParent() 
        {
            Parent.OnParentWindowSet -= AddWidget;
        }

        protected override bool InterferesWith(WidgetAction action)
        {
            return false;
        }

        public void AddWidget(object sender, EventArgs args)
        {
            if (CloneWidgetOnAdd) Parent.ParentWindow.MainWidget.Add((Widget)Widget.Clone());
            else Parent.Add(Widget);
            EndAction();
        }

        public override object InitialClone()
        {
            AddMainWidget c = (AddMainWidget)base.InitialClone();
            if (CloneWidgetInClone) c.Widget = (Widget)Widget.Clone();
            else c.Widget = Widget;
            c.CloneWidgetOnAdd = CloneWidgetOnAdd;
            c.CloneWidgetInClone = CloneWidgetInClone;
            return c;
        }

        protected override bool Matches(WidgetAction action)
        {
            return action is AddMainWidget add_widget && add_widget.Widget == Widget;
        }
    }
}
