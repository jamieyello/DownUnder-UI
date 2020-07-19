using DownUnder.UI.Widgets.Actions.DataTypes;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Actions.Functional
{
    [DataContract] public class AddMainWidget : WidgetAction
    {
        [DataMember] public Widget Widget { get; set; }
        [DataMember] public bool CloneWidgetOnAdd { get; set; } = true;
        [DataMember] public bool CloneWidgetInClone { get; set; } = true;
        [DataMember] public AddNewWidgetLocation LocationOptions { get; set; }

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

        protected override void ConnectEvents() { }
        protected override void DisconnectEvents() 
        {
            Parent.OnParentWindowSet -= AddWidget;
        }

        protected override bool InterferesWith(WidgetAction action)
        {
            return false;
        }

        public void AddWidget(object sender, EventArgs args)
        {
            Widget widget;
            if (CloneWidgetOnAdd) widget = (Widget)Widget.Clone();
            else widget = Widget;
            LocationOptions?.ApplyLocation(Parent, widget);
            Parent.ParentWindow.MainWidget.Add(widget);
            EndAction();
        }

        public override object InitialClone()
        {
            AddMainWidget c = (AddMainWidget)base.InitialClone();
            if (CloneWidgetInClone) c.Widget = (Widget)Widget.Clone();
            else c.Widget = Widget;
            c.CloneWidgetOnAdd = CloneWidgetOnAdd;
            c.CloneWidgetInClone = CloneWidgetInClone;
            c.LocationOptions = (AddNewWidgetLocation)LocationOptions.Clone();
            return c;
        }

        protected override bool Matches(WidgetAction action)
        {
            return action is AddMainWidget add_widget && add_widget.Widget == Widget;
        }
    }
}
