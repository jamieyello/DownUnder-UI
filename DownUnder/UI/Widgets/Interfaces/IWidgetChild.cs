using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Interfaces
{
    internal interface IWidgetChild
    {
        bool IsInitialized { get; }
        Widget Parent { get; set; }
    }
}
