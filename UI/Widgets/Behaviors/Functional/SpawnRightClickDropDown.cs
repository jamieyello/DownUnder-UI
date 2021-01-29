using DownUnder.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Functional
{
    public class SpawnRightClickDropDown : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        protected override void Initialize()
        {
            //Parent.DropDownEntries["Copy"].ClickAction = null;
            //Parent.DropDownEntries["Paste"].ClickAction = null;
        }

        protected override void ConnectEvents()
        {
            Parent.OnRightClick += SpawnDropDown;
            Parent.OnGetDropDownEntries += GetDropDown;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnRightClick -= SpawnDropDown;
            Parent.OnGetDropDownEntries -= GetDropDown;
        }

        public override object Clone()
        {
            SpawnRightClickDropDown c = new SpawnRightClickDropDown();
            return c;
        }

        void GetDropDown(object sender, GetDropDownEntriesArgs args)
        {
            args.DropDowns["Copy"].ClickAction = null;
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
