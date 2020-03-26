using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Interfaces
{
    /// <summary>
    /// Used by serialization to set parent references.
    /// </summary>
    public interface INeedsWidgetParent
    {
        Widget Parent { get; set; }
    }
}
