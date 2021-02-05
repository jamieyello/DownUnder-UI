using DownUnder.UI.Widgets.DataTypes;
using System;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Actions.Functional {
    [DataContract] public sealed class AddMainWidget : WidgetAction {
        [DataMember] public Widget Widget { get; set; }
        [DataMember] public bool CloneWidgetOnAdd { get; set; } = true;
        [DataMember] public bool CloneWidgetInClone { get; set; } = true;
        [DataMember] public OverlayWidgetLocation Location { get; set; }

        public AddMainWidget() {
        }

        public AddMainWidget(
            Widget widget,
            OverlayWidgetLocation location
        ) {
            Widget = widget;
            Location = location;
        }

        protected override void Initialize() {
            if (Parent.ParentDWindow is { })
                AddWidget(this, EventArgs.Empty);
            else Parent.OnParentWindowSet += AddWidget;
        }

        protected override void ConnectEvents() {
        }

        protected override void DisconnectEvents() =>
            Parent.OnParentWindowSet -= AddWidget;

        protected override bool InterferesWith(WidgetAction action) =>
            false;

        public void AddWidget(object sender, EventArgs args) {
            var widget = CloneWidgetOnAdd ? (Widget)Widget.Clone() : Widget;
            Location?.ApplyLocation(Parent, widget);
            Parent.ParentDWindow.MainWidget.Add(widget);
            EndAction();
        }

        public override object InitialClone()
        {
            AddMainWidget c = (AddMainWidget)base.InitialClone();
            if (CloneWidgetInClone) c.Widget = (Widget)Widget.Clone();
            else c.Widget = Widget;
            c.CloneWidgetOnAdd = CloneWidgetOnAdd;
            c.CloneWidgetInClone = CloneWidgetInClone;
            c.Location = (OverlayWidgetLocation)Location.Clone();
            return c;
        }

        protected override bool Matches(WidgetAction action)
        {
            return action is AddMainWidget add_widget && add_widget.Widget == Widget;
        }
    }
}
