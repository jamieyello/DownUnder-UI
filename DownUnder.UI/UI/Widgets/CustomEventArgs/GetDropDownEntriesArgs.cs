using System;

namespace DownUnder.UI.UI.Widgets.CustomEventArgs
{
    public class GetDropDownEntriesArgs : EventArgs
    {
        /// <summary> Add to this <see cref="DropDownEntryList"/> to submit drop down entries. Does not include previously added entries. </summary>
        public readonly DropDownEntryList DropDowns = new DropDownEntryList();
    }
}
