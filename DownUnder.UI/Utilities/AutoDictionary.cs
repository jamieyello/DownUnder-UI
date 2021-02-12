using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DownUnder.UI.Utilities {
    /// <summary> A <see cref="Dictionary{TKey, TValue}"/> that automatically creates new entries on reading nonexistent ones. </summary>
    [DataContract]
    public sealed class AutoDictionary<TKey, TValue> : IAutoDictionary<TKey, TValue> {
        [DataMember] readonly Dictionary<TKey, TValue> _tags = new Dictionary<TKey, TValue>();
        readonly Func<TValue> _create_default_value;

        public int Count => _tags.Count;

        public AutoDictionary() {
        }

        public AutoDictionary(Func<TValue> create_default_value) =>
            _create_default_value = create_default_value;

        public AutoDictionary(
            IEnumerable<KeyValuePair<TKey, TValue>> entries,
            Func<TValue> create_default_value = null
        ) {
            _tags = new Dictionary<TKey, TValue>(entries);
            _create_default_value = create_default_value;
        }

        public TValue this[TKey key] {
            get {
                if (_tags.TryGetValue(key, out var result))
                    return result;

                if (_create_default_value is { }) {
                    var default_value = _create_default_value.Invoke();
                    _tags.Add(key, default_value);
                    return default_value;
                }

                var has_parameterless = false;
                foreach (var constructor in typeof(TValue).GetConstructors())
                    has_parameterless |= !constructor.GetParameters().Any();

                if (!has_parameterless)
                    throw new Exception($"Value not found, no create_default_value parameter was given, and {typeof(TValue).Name} does not have a parameterless constructor.");

                if (Activator.CreateInstance(typeof(TValue)) is not TValue new_entry)
                    throw new Exception($"Failed to create an instance of type '{typeof(TValue).Name}'.");

                _tags.Add(key, new_entry);
                return new_entry;
            } set {
                if (_tags.ContainsKey(key))
                    _tags.Remove(key);

                _tags.Add(key, value);
            }
        }

        public void Add(TKey key, TValue value, bool replace = false) {
            var kvp = (key, value).ToKVP();
            Add(kvp, replace);
        }

        public void Add(
            IEnumerable<KeyValuePair<TKey, TValue>> pairs,
            bool replace = false
        ) {
            foreach (var pair in pairs)
                Add(pair, replace);
        }

        public void Add(KeyValuePair<TKey, TValue> pair, bool replace = false) {
            var (key, value) = pair;

            if (replace)
                this[key] = value;
            else if (!_tags.ContainsKey(pair.Key))
                _tags.Add(pair.Key, pair.Value);
        }

        public bool Remove(TKey key) => _tags.Remove(key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _tags.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _tags.GetEnumerator();
    }

    public static class AutoDictionary {
        public static AutoDictionary<TKey, TValue> Create<TKey, TValue>(
            IEnumerable<KeyValuePair<TKey, TValue>> kvps
        ) =>
            new AutoDictionary<TKey, TValue>(kvps);
    }
}