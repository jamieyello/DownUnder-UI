using DownUnder.UI.UI;
using DownUnder.UI.UI.Widgets.Behaviors;
using DownUnder.UI.UI.Widgets.CustomEventArgs;

namespace DownUnder.UI.Editor.Behaviors
{
    public class GetEditModeDropDowns : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        DropDownEntryList entries = new DropDownEntryList();

        protected override void Initialize()
        {
            entries["Copy"].ClickAction = null;
            entries["Cut"].ClickAction = null;
            entries["Paste"].ClickAction = null;

            DropDownEntryList side_menu = new DropDownEntryList();
            side_menu["OnClick"].ClickAction = null;
            side_menu["OnRightClick"].ClickAction = null;
            entries["Go to Slot"].SideDropDown = side_menu;
        }

        protected override void ConnectEvents()
        {
            Parent.OnGetDropDownEntries += GetDropDowns;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnGetDropDownEntries -= GetDropDowns;
        }

        public override object Clone()
        {
            GetEditModeDropDowns c = new GetEditModeDropDowns();
            return c;
        }

        void GetDropDowns(object sender, GetDropDownEntriesArgs args)
        {
            args.DropDowns.Add(entries);
        }
    }
}
