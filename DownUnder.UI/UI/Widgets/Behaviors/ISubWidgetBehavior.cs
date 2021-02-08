namespace DownUnder.UI.UI.Widgets.Behaviors {
    public interface ISubWidgetBehavior<out TBase> where TBase : WidgetBehavior {
        TBase BaseBehavior { get; }
    }
}