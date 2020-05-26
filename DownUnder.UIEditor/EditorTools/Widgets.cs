using DownUnder.UI;
using DownUnder.UI.Widgets;
using DownUnder.UI.Widgets.Behaviors.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UIEditor.EditorTools
{
    public static class EditorWidgets
    {
        public static Widget UIEditor(DWindow parent)
        {
            GridFormat main_grid = new GridFormat(2, 1);
            Widget layout = new Widget(parent);
            layout.Behaviors.Add(main_grid);

            return layout;
        }
    }
}
