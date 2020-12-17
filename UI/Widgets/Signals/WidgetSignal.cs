using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets
{
    public class WidgetSignal
    {
        public enum ReplyType
        {
            reject,
            accept,
            ignore
        }

        public ReplyType Reply = ReplyType.accept;
        public string ErrorMessage = "";
    }
}
