using DownUnder.UI.Widgets.Actions;
using DownUnder.UI.Widgets.Behaviors.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets
{
    [DataContract]
    public class DropDownEntry : ICloneable
    {
        /// <summary> The <see cref="WidgetAction"/> that should be done when this entry is selected. </summary>
        [DataMember] public WidgetAction ClickAction { get; set; }
        /// <summary> When set to a value not null, the <see cref="ClickAction"/> will be ignored and will instead create a new menu. </summary>
        [DataMember] public DropDownEntryList SideDropDown { get; set; }

        object ICloneable.Clone() => Clone();
        public DropDownEntry Clone()
        {
            DropDownEntry c = new DropDownEntry();
            c.ClickAction = (WidgetAction)ClickAction.InitialClone();
            return c;
        }
    }
}
