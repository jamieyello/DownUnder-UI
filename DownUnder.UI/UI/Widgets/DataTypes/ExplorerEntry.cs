using DownUnder.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;

namespace DownUnder.UI.Widgets.DataTypes
{
    [DataContract]
    public class ExplorerEntry : IAutoDictionary<string, ExplorerEntry>
    {
        public enum ExplorerEntryType
        {
            file,
            folder
        }

        [DataMember] public ExplorerEntryType EntryType { get; set; }
        [DataMember] public AutoDictionary<string, ExplorerEntry> ChildEntries = new AutoDictionary<string, ExplorerEntry>();

        public ExplorerEntry() { }
        public ExplorerEntry(IEnumerable<KeyValuePair<string, ExplorerEntry>> files)
        {
            EntryType = ExplorerEntryType.folder;
            ChildEntries = new AutoDictionary<string, ExplorerEntry>(files);
        }

        public ExplorerEntry GetDirectories() =>
            new ExplorerEntry(
                from KeyValuePair<string, ExplorerEntry> entry in this
                where entry.Value.EntryType == ExplorerEntryType.folder
                select entry);

        public ExplorerEntry GetFiles() =>
            new ExplorerEntry(
                from KeyValuePair<string, ExplorerEntry> entry in this
                where entry.Value.EntryType == ExplorerEntryType.file
                select entry);

        public static ExplorerEntry FromFolder(string path)
        {
            if (!Directory.Exists(path)) throw new Exception("Not a valid path.");
            ExplorerEntry result = new ExplorerEntry() { EntryType = ExplorerEntryType.folder };

            foreach (string entry in Directory.GetDirectories(path)) result[Path.GetFileName(entry)] = FromFolder(entry);
            foreach (string entry in Directory.GetFiles(path)) result[Path.GetFileName(entry)].EntryType = ExplorerEntryType.file;

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
