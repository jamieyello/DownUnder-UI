using DownUnder.UI.Widgets.DataTypes;
using DownUnder.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.Behaviors.Functional
{
    public class DropDownBase : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };
        public AutoDictionary<string, DropDownEntry> Entries { get; private set; } = new AutoDictionary<string, DropDownEntry>();

        protected override void Initialize() { }

        protected override void ConnectEvents() { }

        protected override void DisconnectEvents() { }

        public override object Clone()
        {
            DropDownBase c = new DropDownBase();
            c.Entries = Entries.Clone();
            return c;
        }
    }
}
