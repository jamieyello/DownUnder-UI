using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using DownUnder.UI.Utilities;
using static DownUnder.UI.Widgets.DataTypes.ExplorerEntry.ExplorerEntryType;

namespace DownUnder.UI.Widgets.DataTypes {
    [DataContract]
    public sealed class ExplorerEntry : IAutoDictionary<string, ExplorerEntry> {
        public enum ExplorerEntryType {
            file,
            folder
        }

        [DataMember] public ExplorerEntryType EntryType { get; set; }
        [DataMember] public AutoDictionary<string, ExplorerEntry> ChildEntries { get; } = new AutoDictionary<string, ExplorerEntry>();

        public ExplorerEntry() {
        }

        public ExplorerEntry(
            IEnumerable<KeyValuePair<string, ExplorerEntry>> files
        ) {
            EntryType = folder;
            ChildEntries = new AutoDictionary<string, ExplorerEntry>(files);
        }

        public ExplorerEntry GetDirectories() =>
            new ExplorerEntry(this.Where(entry => entry.Value.EntryType == folder));

        public ExplorerEntry GetFiles() =>
            new ExplorerEntry(this.Where(entry => entry.Value.EntryType == file));

        public static ExplorerEntry FromFolder(string path) {
            if (!Directory.Exists(path))
                throw new Exception("Not a valid path.");

            var result = new ExplorerEntry { EntryType = folder };

            foreach (var entry in Directory.GetDirectories(path))
                result[Path.GetFileName(entry)] = FromFolder(entry);

            foreach (var entry in Directory.GetFiles(path))
                result[Path.GetFileName(entry)].EntryType = file;

            return result;
        }

        #region IAutoDictionary

        public int Count => ChildEntries.Count;
        public ExplorerEntry this[string key] { get => ChildEntries[key]; set => ChildEntries[key] = value; }
        public void Add(string key, ExplorerEntry value, bool replace = false) => ChildEntries.Add(key, value, replace);
        public void Add(KeyValuePair<string, ExplorerEntry> pair, bool replace = false) => ChildEntries.Add(pair, replace);
        public void Add(IEnumerable<KeyValuePair<string, ExplorerEntry>> pairs, bool replace = false) => ChildEntries.Add(pairs, replace);
        public bool Remove(string key) => ChildEntries.Remove(key);
        public IEnumerator<KeyValuePair<string, ExplorerEntry>> GetEnumerator() => ChildEntries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ChildEntries.GetEnumerator();

        #endregion
    }
}
