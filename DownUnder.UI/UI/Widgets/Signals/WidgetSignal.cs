using System;
using static DownUnder.UI.UI.Widgets.Signals.WidgetResponse;

namespace DownUnder.UI.UI.Widgets.Signals
{
    public class WidgetSignal
    {
        Action<WidgetResponse> response_handle;

        public WidgetSignal(Action<WidgetResponse> response_handle)
        {
            this.response_handle = response_handle;
        }

        public void SendResponse(ResponseType response, string message = "")
        {
            response_handle.Invoke(new WidgetResponse(response, message));
        }
    }
}
