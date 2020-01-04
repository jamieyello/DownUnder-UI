using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.Utilities
{
    public static class OSInterface
    {
        public static void CopyToClipBoard(string text)
        {
            System.Windows.Forms.Clipboard.SetText(text);
        }

        public static string GetTextFromClipboard()
        {
            return System.Windows.Forms.Clipboard.GetText();
        }
    }
}
