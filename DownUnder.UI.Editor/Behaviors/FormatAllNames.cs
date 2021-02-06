using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors;
using System.Collections.Generic;
using System.Linq;

namespace DownUnder.UIEditor.Behaviors
{
    public class FormatAllNames : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };
        bool disable_events = false;

        protected override void Initialize()
        {

        }

        protected override void ConnectEvents()
        {
            Parent.OnAddChildAny += HandleNewWidget;
            Parent.OnRenameAny += HandleRename;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnAddChildAny -= HandleNewWidget;
            Parent.OnRenameAny -= HandleRename;
        }

        public override object Clone()
        {
            FormatAllNames c = new FormatAllNames();
            return c;
        }

        void HandleRename(object sender, RenameArgs args)
        {
            if (disable_events) return;
            disable_events = true;
            args.QuietRename = GetFormattedName(args.Widget);
            disable_events = false;
        }

        void HandleNewWidget(object sender, WidgetArgs args)
        {
            if (disable_events) return;
            disable_events = true;
            string old = args.Widget.Name;
            args.Widget.Name = GetFormattedName(args.Widget);
            //Debug.WriteLine("FANB HandleNewWidget old name {" + old + "}, new name = {" + args.Widget.Name + "}");
            disable_events = false;
        }

        string GetFormattedName(Widget widget)
        {
            List<Widget> widgets = Parent.AllContainedWidgets;
            widgets.Remove(widget);
            widgets.Remove(Parent);
            string new_name = widget.Name;
            int i = 0;
            while ((from Widget match in widgets
                    where match.Name == new_name
                    select match).Count() > 0)
                new_name = widget.Name + i++;
            return new_name;
        }
    }
}
