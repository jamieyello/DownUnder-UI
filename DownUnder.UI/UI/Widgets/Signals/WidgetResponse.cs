namespace DownUnder.UI.UI.Widgets.Signals
{
    public class WidgetResponse
    {
        public enum ResponseType
        {
            reject,
            ignore,
            accept
        }

        public WidgetResponse(ResponseType response, string message)
        {
            Reply = response;
            Message = message;
        }

        public readonly ResponseType Reply;
        public readonly string Message;
    }
}
