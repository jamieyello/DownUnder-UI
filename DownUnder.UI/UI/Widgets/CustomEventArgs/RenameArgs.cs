using System;

namespace DownUnder.UI.UI.Widgets.CustomEventArgs
{
    public class RenameArgs : EventArgs
    {
        /// <summary> The <see cref="Widget"/> that has been renamed. </summary>
        public readonly Widget Widget;
        public readonly string OldName;
        public readonly string NewName;
        /// <summary> Set this to force the name without invoking any additional logic. This may break things, and will be removed in the future. </summary>
        public string QuietRename = null;

        public RenameArgs(Widget widget, string old, string new_)
        {
            Widget = widget;
            OldName = old;
            NewName = new_;
        }
    }
}
