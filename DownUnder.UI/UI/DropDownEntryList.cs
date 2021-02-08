using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DownUnder.UI.UI.Widgets;
using DownUnder.UI.Utilities;

namespace DownUnder.UI.UI {
    [DataContract]
    public sealed class DropDownEntryList : IAutoDictionary<string, DropDownEntry> {
        [DataMember] readonly IAutoDictionary<string, DropDownEntry> _entries;

        public DropDownEntry this[string key] {
            get => _entries[key];
            set => _entries[key] = value;
        }

        public DropDownEntryList() =>
            _entries = new AutoDictionary<string, DropDownEntry>();

        public DropDownEntryList(
            IEnumerable<KeyValuePair<string, DropDownEntry>> entries
        ) =>
            _entries = new AutoDictionary<string, DropDownEntry>(entries);

        public DropDownEntryList Clone() =>
            new DropDownEntryList(this);

        #region IAutoDictionary

        public void Add(string key, DropDownEntry value, bool replace = false) => _entries.Add(key, value, replace);
        public void Add(KeyValuePair<string, DropDownEntry> pair, bool replace = false) => _entries.Add(pair, replace);
        public void Add(IEnumerable<KeyValuePair<string, DropDownEntry>> pairs, bool replace = false) => _entries.Add(pairs, replace);
        public bool Remove(string key) => _entries.Remove(key);
        public IEnumerator<KeyValuePair<string, DropDownEntry>> GetEnumerator() => _entries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();
        public int Count => _entries.Count;

        #endregion
    }
}
