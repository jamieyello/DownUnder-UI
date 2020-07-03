using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DownUnder.Utilities
{
    /// <summary> A <see cref="Dictionary{TKey, TValue}"/> that automatically creates new entries on reading nonexistent ones. </summary>
    [DataContract] public class AutoDictionary<TKey, TValue> : ICloneable
    {
        [DataMember] Dictionary<TKey, TValue> _tags = new Dictionary<TKey, TValue>();
        private readonly Func<TValue> _create_default_value;

        public AutoDictionary() { }
        public AutoDictionary(Func<TValue> create_default_value) {
            _create_default_value = create_default_value;
        }

        public TValue this[TKey key] {
            get {
                if (_tags.TryGetValue(key, out TValue result)) return result;
                if (_create_default_value != null)
                {
                    TValue new_entry = _create_default_value.Invoke();
                    _tags.Add(key, new_entry);
                    return new_entry;
                }
                throw new Exception($"Value not found and no create_default_value parameter was given.");
            }
            set {
                if (_tags.ContainsKey(key)) _tags.Remove(key);
                _tags.Add(key, value);
            }
        }

        public object Clone()
        {
            AutoDictionary<TKey, TValue> c = new AutoDictionary<TKey, TValue>(_create_default_value);
            c._tags = new Dictionary<TKey, TValue>(_tags);
            return c;
        }
    }
}