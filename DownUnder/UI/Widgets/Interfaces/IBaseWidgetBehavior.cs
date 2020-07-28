using DownUnder.UI.Widgets.Behaviors.DataTypes;

namespace DownUnder.UI.Widgets.Interfaces
{
    public interface IBaseWidgetBehavior
    {
        GroupBehaviorPolicy SubWidgetBehavior { get; set; }
    }
}
