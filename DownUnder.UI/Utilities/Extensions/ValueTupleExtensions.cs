using System.Collections.Generic;

namespace DownUnder.UI {
    public static class ValueTupleExtensions {
        public static KeyValuePair<TKey, TValue> ToKVP<TKey, TValue>(
            this (TKey Key, TValue Value) me
        ) =>
            new KeyValuePair<TKey,TValue>(me.Key, me.Value);
    }
}
