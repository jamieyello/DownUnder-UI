using System;

namespace DownUnder.UI.Utilities.CustomEventArgs
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
