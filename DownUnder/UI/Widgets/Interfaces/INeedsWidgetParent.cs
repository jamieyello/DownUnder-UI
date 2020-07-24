namespace DownUnder.UI.Widgets.Interfaces {
    /// <summary> Used by serialization to set parent references. </summary>
    public interface IIsWidgetChild {
        Widget Parent { get; set; }
    }
}
