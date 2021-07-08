using DownUnder.UI.Widgets;
using System;

namespace DownUnder.UI {
    public sealed class RenameArgs : EventArgs {
        /// <summary> The <see cref="Widget"/> that has been renamed. </summary>
        public Widget Widget { get; }
        public string OldName { get; }
        public string NewName { get; }

        /// <summary> Set this to force the name without invoking any additional logic. This may break things, and will be removed in the future. </summary>
        public string QuietRename { get; set; }

        public RenameArgs(Widget widget, string old, string new_) {
            Widget = widget;
            OldName = old;
            NewName = new_;
        }
    }
}
