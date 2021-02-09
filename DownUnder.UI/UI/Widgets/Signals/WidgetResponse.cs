namespace DownUnder.UI.UI.Widgets.Signals {
    public sealed class WidgetResponse {
        public ResponseType Reply { get; }
        public string Message { get; }

        public enum ResponseType {
            reject,
            ignore,
            accept
        }

        public WidgetResponse(
            ResponseType response,
            string message
        ) {
            Reply = response;
            Message = message;
        }
    }
}
