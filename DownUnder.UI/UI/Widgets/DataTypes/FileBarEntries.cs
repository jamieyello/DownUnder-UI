using DownUnder.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DownUnder.UI.Widgets.DataTypes
{
    public class FileBarEntries : ICloneable, IEnumerable<AutoDictionary<string, DropDownEntry>>
    {
        AutoDictionary<string, AutoDictionary<string, DropDownEntry>> file_bar_entries = new AutoDictionary<string, AutoDictionary<string, DropDownEntry>>();

        public AutoDictionary<string, DropDownEntry> this[string entry]
            => file_bar_entries[entry];

        object ICloneable.Clone() => Clone();
        public FileBarEntries Clone()
        {
            FileBarEntries c = new FileBarEntries();
            c.file_bar_entries =  new AutoDictionary<string, AutoDictionary<string, DropDownEntry>>(file_bar_entries);
            return c;
        }

        public bool Remove(string key) => file_bar_entries.Remove(key);

        public IEnumerator<AutoDictionary<string, DropDownEntry>> GetEnumerator()
        {
            return ((IEnumerable<AutoDictionary<string, DropDownEntry>>)file_bar_entries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)file_bar_entries).GetEnumerator();
        }
    }
}
