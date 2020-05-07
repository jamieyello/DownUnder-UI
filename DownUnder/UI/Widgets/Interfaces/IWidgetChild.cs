namespace DownUnder.UI.Widgets.Interfaces {
    internal interface IWidgetChild {
        bool IsInitialized { get; }
        Widget Parent { get; set; }
    }
}
