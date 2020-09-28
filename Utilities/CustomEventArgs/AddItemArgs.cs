using System;

namespace DownUnder.Utilities
{
    public class AddItemArgs<T> : EventArgs
    {
        public AddItemArgs(T item)
        {
            LastAddedItem = item;
        }

        public T LastAddedItem;
    }
}
