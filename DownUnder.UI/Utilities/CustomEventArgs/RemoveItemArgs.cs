using System;

namespace DownUnder.UI.Utilities.CustomEventArgs
{
    public class RemoveItemArgs<T> : EventArgs
    {
        public RemoveItemArgs(T item)
        {
            LastRemovedItem = item;
        }

        public T LastRemovedItem;
    }
}
