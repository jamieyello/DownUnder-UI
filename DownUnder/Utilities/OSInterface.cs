using DownUnder.UI;
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

        public static float CaretBlinkTime
        {
            get => System.Windows.Forms.SystemInformation.CaretBlinkTime / 500f;
        }

        public static void SetWindowPosition(DWindow d_window, Point position)
        {
            ((System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(d_window.Window.Handle))
                    .Location = new System.Drawing.Point((int)position.X, (int)position.Y);
        }
    }
}
