using DownUnder.UI.Widgets.Actions;
using DownUnder.UI.Widgets.Behaviors.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets
{
    public class DropDownEntry : ICloneable
    {
        public WidgetAction ClickAction { get; set; }

        object ICloneable.Clone() => Clone();
        public DropDownEntry Clone()
        {
            DropDownEntry c = new DropDownEntry();
            c.ClickAction = (WidgetAction)ClickAction.InitialClone();
            return c;
        }
    }
}
