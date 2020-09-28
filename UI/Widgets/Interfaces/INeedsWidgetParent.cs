namespace DownUnder.UI.Widgets.Interfaces {
    /// <summary> Used by serialization to set parent references. </summary>
    internal interface IIsWidgetChild {
        Widget Parent { get; set; }
    }
}
