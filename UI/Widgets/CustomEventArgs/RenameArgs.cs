using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets
{
    public class RenameArgs : EventArgs
    {
        /// <summary> The <see cref="Widget"/> that has been renamed. </summary>
        public readonly Widget Widget;
        public readonly string OldName;
        public readonly string NewName;

        public RenameArgs(Widget widget, string old, string new_)
        {
            Widget = widget;
            OldName = old;
            NewName = new_;
        }
    }
}
