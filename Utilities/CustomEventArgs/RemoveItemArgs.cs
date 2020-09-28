using System;

namespace DownUnder.Utilities
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
