using System;
using System.Collections;
using System.Collections.Generic;

namespace DownUnder.Utilities
{
    /// <summary> A list that invokes OnAdd and OnRemove <see cref="EventHandler"/>s. </summary>
    public class EventList<T> : IList<T>
    {
        public event EventHandler<AddItemArgs<T>> OnAdd;
        public event EventHandler<RemoveItemArgs<T>> OnRemove;

        private List<T> _list = new List<T>();

        public T this[int index] { 
            get => ((IList<T>)_list)[index];
            set {
                T removed = _list[index];
                _list[index] = value;
                OnRemove.Invoke(this, new RemoveItemArgs<T>(removed));
                OnAdd.Invoke(this, new AddItemArgs<T>(value));
            }
        }

        public int Count => _list.Count;

        public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

        public void Add(T item) {
            _list.Add(item);
            OnAdd.Invoke(this, new AddItemArgs<T>(item));
        }

        public void Clear() {
            int c = Count;
            for (int i = 0; i < c; i++) RemoveAt(0);
        }

        public bool Contains(T item) => _list.Contains(item);
        
        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        
        public int IndexOf(T item) => _list.IndexOf(item);
        
        public void Insert(int index, T item) {
            _list.Insert(index, item);
            OnAdd.Invoke(this, new AddItemArgs<T>(item));
        }

        public bool Remove(T item) {
            bool result = _list.Remove(item);
            OnRemove.Invoke(this, new RemoveItemArgs<T>(item));
            return result;
        }

        public void RemoveAt(int index) {
            T removed = _list[index];
            _list.RemoveAt(index);
            OnRemove.Invoke(this, new RemoveItemArgs<T>(removed));
        }

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}
