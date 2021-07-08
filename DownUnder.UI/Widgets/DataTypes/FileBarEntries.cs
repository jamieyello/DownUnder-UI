using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DownUnder.UI.Utilities;

namespace DownUnder.UI.Widgets.DataTypes {
    public sealed class FileBarEntries : ICloneable, IEnumerable<AutoDictionary<string, DropDownEntry>> {
        readonly AutoDictionary<string, AutoDictionary<string, DropDownEntry>> file_bar_entries = new AutoDictionary<string, AutoDictionary<string, DropDownEntry>>();

        public AutoDictionary<string, DropDownEntry> this[string entry] => file_bar_entries[entry];

        public FileBarEntries() {
        }

        public FileBarEntries(
            FileBarEntries source
        ) =>
            file_bar_entries = AutoDictionary.Create(source.file_bar_entries);

        public bool Remove(string key) =>
            file_bar_entries.Remove(key);

        object ICloneable.Clone() => Clone();
        public FileBarEntries Clone() => new FileBarEntries(this);

        public IEnumerator<AutoDictionary<string, DropDownEntry>> GetEnumerator() => file_bar_entries.Select(kvp => kvp.Value).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}