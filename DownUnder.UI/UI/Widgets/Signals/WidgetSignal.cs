using System;
using static DownUnder.UI.UI.Widgets.Signals.WidgetResponse;

namespace DownUnder.UI.UI.Widgets.Signals {
    public class WidgetSignal {
        readonly Action<WidgetResponse> _response_handle;

        public WidgetSignal(Action<WidgetResponse> response_handle) =>
            _response_handle = response_handle;

        public void SendResponse(ResponseType response, string message = "") =>
            _response_handle.Invoke(new WidgetResponse(response, message));
    }
}
