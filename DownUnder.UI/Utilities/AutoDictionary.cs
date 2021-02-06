using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DownUnder.Utilities
{
    /// <summary> A <see cref="Dictionary{TKey, TValue}"/> that automatically creates new entries on reading nonexistent ones. </summary>
    [DataContract] public class AutoDictionary<TKey, TValue>
        : IAutoDictionary<TKey, TValue>
    {
        [DataMember] Dictionary<TKey, TValue> _tags = new Dictionary<TKey, TValue>();
        private readonly Func<TValue> _create_default_value;

        public int Count => _tags.Count;

        public AutoDictionary() { }
        public AutoDictionary(Func<TValue> create_default_value) {
            _create_default_value = create_default_value;
        }
        public AutoDictionary(IEnumerable<KeyValuePair<TKey, TValue>> entries, Func<TValue> create_default_value = null)
        {
            _tags = new Dictionary<TKey, TValue>();
            foreach (var tag in _tags) _tags.Add(tag.Key, tag.Value);
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

                bool has_parameterless = false;
                foreach (var constructor in typeof(TValue).GetConstructors()) has_parameterless |= constructor.GetParameters().Count() == 0;
                if (has_parameterless)
                {
                    if (Activator.CreateInstance(typeof(TValue)) is TValue new_entry)
                    {
                        _tags.Add(key, new_entry);
                        return new_entry;
                    }
                }

                throw new Exception($"Value not found, no create_default_value parameter was given, and {typeof(TValue).Name} does not have a parameterless constructor.");
            }
            set {
                if (_tags.ContainsKey(key)) _tags.Remove(key);
                _tags.Add(key, value);
            }
        }

        public void Add(TKey key, TValue value, bool replace = false)
        {
            KeyValuePair<TKey, TValue> entry = new KeyValuePair<TKey, TValue>(key, value);
            Add(entry, replace);
        }

        public void Add(IEnumerable<KeyValuePair<TKey, TValue>> pairs, bool replace = false)
        {
            foreach (var pair in pairs) Add(pair, replace);
        }

        public void Add(KeyValuePair<TKey, TValue> pair, bool replace = false)
        {
            if (replace) this[pair.Key] = pair.Value;
            else if (!_tags.ContainsKey(pair.Key)) _tags.Add(pair.Key, pair.Value);
        }

        public bool Remove(TKey key) => _tags.Remove(key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _tags.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _tags.GetEnumerator();
    }
}