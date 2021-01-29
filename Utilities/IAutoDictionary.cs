using System;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.Utilities
{
    public interface IAutoDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        TValue this[TKey key] { get; set; }
        void Add(TKey key, TValue value, bool replace = false);
        void Add(KeyValuePair<TKey, TValue> pair, bool replace = false);
        void Add(IEnumerable<KeyValuePair<TKey, TValue>> pairs, bool replace = false);
        bool Remove(TKey key);
        int Count { get; }
    }
}
