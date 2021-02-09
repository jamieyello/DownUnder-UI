using System;
using System.Runtime.Serialization;
using DownUnder.UI.UI.Widgets.Actions;

namespace DownUnder.UI.UI.Widgets {
    [DataContract]
    public sealed class DropDownEntry : ICloneable {
        /// <summary> The <see cref="WidgetAction"/> that should be done when this entry is selected. </summary>
        [DataMember] public WidgetAction ClickAction { get; set; }

        /// <summary> When set to a value not null, the <see cref="ClickAction"/> will be ignored and will instead create a new menu. </summary>
        [DataMember] public DropDownEntryList SideDropDown { get; set; }

        public DropDownEntry() {
        }

        DropDownEntry(
            DropDownEntry source
        ) =>
            ClickAction = (WidgetAction)source.ClickAction.InitialClone();

        object ICloneable.Clone() => Clone();
        public DropDownEntry Clone() => new DropDownEntry(this);
    }
}
