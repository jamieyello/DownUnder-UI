using DownUnder.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Functional
{
    class SpawnRightClickDropDown : WidgetBehavior, ISubWidgetBehavior<DropDownBase>
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        public DropDownBase BaseBehavior => Parent.Behaviors.Get<DropDownBase>();
        
        protected override void Initialize()
        {
            BaseBehavior.Entries["Copy"].ClickAction = null;
            BaseBehavior.Entries["Paste"].ClickAction = null;
        }

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
            if (BaseBehavior.Entries.Count == 0) return;
            Widget drop_down = CommonWidgets.DropDown(BaseBehavior.Entries);
            drop_down.Position = Parent.InputState.CursorPosition;
            Parent.ParentDWindow.MainWidget.Add(drop_down);
        }
    }
}
