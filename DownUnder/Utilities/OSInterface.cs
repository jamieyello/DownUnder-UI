using Microsoft.Xna.Framework;

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

        public static void SetMinimumWindowSize(GameWindow window, Point size)
        {
            System.Windows.Forms.Control.FromHandle(window.Handle).MinimumSize = size.ToSystemSize();
        }
    }
}
