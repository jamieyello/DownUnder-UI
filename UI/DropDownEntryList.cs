using DownUnder.UI.Widgets;
using DownUnder.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DownUnder.UI
{
    [DataContract]
    public class DropDownEntryList : ICloneable, IAutoDictionary<string, DropDownEntry>
    {
        [DataMember] AutoDictionary<string, DropDownEntry> entries = new AutoDictionary<string, DropDownEntry>();

        public DropDownEntry this[string key] { get => ((IAutoDictionary<string, DropDownEntry>)entries)[key]; set => ((IAutoDictionary<string, DropDownEntry>)entries)[key] = value; }

        public DropDownEntryList() { }
        public DropDownEntryList(IEnumerable<KeyValuePair<string, DropDownEntry>> entries)
        {
            entries = new AutoDictionary<string, DropDownEntry>(entries);
        }

        object ICloneable.Clone() => Clone();
        public DropDownEntryList Clone()
        {
            DropDownEntryList c = new DropDownEntryList();
            foreach (var entry in entries) c.entries.Add(entry.Key, entry.Value.Clone());
            return c;
        }

        #region IAutoDictionary

        public void Add(string key, DropDownEntry value, bool replace = false) => entries.Add(key, value, replace);
        public void Add(KeyValuePair<string, DropDownEntry> pair, bool replace = false) => entries.Add(pair, replace);
        public void Add(IEnumerable<KeyValuePair<string, DropDownEntry>> pairs, bool replace = false) => entries.Add(pairs, replace);
        public bool Remove(string key) => entries.Remove(key);
        public IEnumerator<KeyValuePair<string, DropDownEntry>> GetEnumerator() => entries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => entries.GetEnumerator();
        public int Count => entries.Count;
        #endregion
    }
}
