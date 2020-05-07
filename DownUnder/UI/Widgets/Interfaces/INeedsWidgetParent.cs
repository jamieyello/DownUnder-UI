namespace DownUnder.UI.Widgets.Interfaces {
    /// <summary> Used by serialization to set parent references. </summary>
    public interface INeedsWidgetParent {
        Widget Parent { get; set; }
    }
}
