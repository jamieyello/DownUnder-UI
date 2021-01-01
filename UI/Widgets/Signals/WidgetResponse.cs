using System;
using System.Collections.Generic;
using System.Text;
using static DownUnder.UI.Widgets.LoginSignal;

namespace DownUnder.UI.Widgets
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
