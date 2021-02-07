using System;

namespace DownUnder.UI.UI.Widgets.Behaviors.Functional
{
    public class SpawnRightClickDropDown : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        protected override void Initialize() { }

        protected override void ConnectEvents()
        {
            Parent.OnRightClick += SpawnDropDown;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnRightClick -= SpawnDropDown;
        }

        public override object Clone()
        {
            SpawnRightClickDropDown c = new SpawnRightClickDropDown();
            return c;
        }

        void SpawnDropDown(object sender, EventArgs args)
        {
            DropDownEntryList drop_downs = Parent.InvokeOnGetDropDownEntries();
            if (drop_downs.Count == 0) return;

            Widget drop_down = CommonWidgets.DropDown(drop_downs);
            drop_down.Position = Parent.InputState.CursorPosition;
            Parent.ParentDWindow.MainWidget.Add(drop_down);
        }
    }
}
