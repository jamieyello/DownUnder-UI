namespace DownUnder.UI.Widgets.Behaviors
{
    /// <summary> Describes a <see cref="WidgetBehavior"/> that implements additional behaviors to <see cref="Widget.Children"/>. </summary>
    public interface IHostWidgetBehavior
    {
        GroupBehaviorPolicy SubWidgetBehavior { get; set; }
    }
}
