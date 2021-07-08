using System;

namespace DownUnder.UI.Widgets.Behaviors.Functional {
    sealed class SpawnRightClickDropDown : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.FUNCTION };

        protected override void Initialize() {
        }

        protected override void ConnectEvents() =>
            Parent.OnRightClick += SpawnDropDown;

        protected override void DisconnectEvents() =>
            Parent.OnRightClick -= SpawnDropDown;

        public override object Clone() =>
            new SpawnRightClickDropDown();

        void SpawnDropDown(object sender, EventArgs args) {
            var drop_downs = Parent.InvokeOnGetDropDownEntries();
            if (drop_downs.Count == 0)
                return;

            var drop_down = CommonWidgets.DropDown(drop_downs);
            drop_down.Position = Parent.InputState.CursorPosition;
            Parent.ParentDWindow.MainWidget.Add(drop_down);
        }
    }
}