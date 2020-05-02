namespace DownUnder.UI.Widgets.Behaviors {
    public class BehaviorLibraryAccessor {
        /// <summary> A collection of <see cref="WidgetBehavior"/>s that only add visual features. </summary>
        public VisualBehaviorLibraryAccessor Visual => new VisualBehaviorLibraryAccessor();
        /// <summary> A collection of <see cref="WidgetBehavior"/>s that will change the way the <see cref="Widget"/> behaves. </summary>
        public ActiveBehaviorLibraryAccessor Active => new ActiveBehaviorLibraryAccessor();
    }
}
