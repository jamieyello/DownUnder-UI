using System;

namespace DownUnder.UI
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
