using DownUnder.UI.Widgets.Behaviors.DataTypes;
using DownUnder.UI.Widgets.DataTypes;
using System.Collections.Generic;

namespace DownUnder.UI.Widgets.Behaviors
{
    /// <summary> Describes a <see cref="WidgetBehavior"/> that implements additional behaviors. </summary>
    public interface IHostWidgetBehavior
    {
        GroupBehaviorPolicy SubWidgetBehavior { get; set; }
    }
}
