using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DownUnder.Utilities
{
    /// <summary> A <see cref="Dictionary{TKey, TValue}"/> that automatically creates new entries on reading nonexistent ones. </summary>
    [DataContract] public class AutoDictionary<TKey, TValue> : ICloneable, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        [DataMember] Dictionary<TKey, TValue> _tags = new Dictionary<TKey, TValue>();
        private readonly Func<TValue> _create_default_value;

        public int Count => _tags.Count;

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

        public bool Remove(TKey key) => _tags.Remove(key);

        object ICloneable.Clone() => Clone();
        public AutoDictionary<TKey, TValue> Clone()
        {
            AutoDictionary<TKey, TValue> c = new AutoDictionary<TKey, TValue>(_create_default_value);
            c._tags = new Dictionary<TKey, TValue>(_tags);
            return c;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)_tags).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_tags).GetEnumerator();
        }
    }
}