using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DownUnder.UIEditor
{
    class OSInterface : IOSInterface
    {
        public float CaretBlinkTime => 1f;

        public void CopyTextToClipBoard(string text) => System.Windows.Forms.Clipboard.SetText(text);

        public string GetTextFromClipboard() => System.Windows.Forms.Clipboard.GetText();

        public string ReadAllText(string path) => File.ReadAllText(path);

        public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);
    }
}
