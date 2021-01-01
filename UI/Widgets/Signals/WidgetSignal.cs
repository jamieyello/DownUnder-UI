using System;
using System.Collections.Generic;
using System.Text;
using static DownUnder.UI.Widgets.WidgetResponse;

namespace DownUnder.UI.Widgets
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
