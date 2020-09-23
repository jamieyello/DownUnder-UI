using DownUnder.UI;
using Microsoft.Xna.Framework;
using TextCopy;

namespace DownUnder.Utilities {
    public static class OSInterface {
        public static void CopyTextToClipBoard(string text) => ClipboardService.SetText(text);
        //public static void CopyObjectToClipboard(object obj, bool copy) => System.Windows.Forms.Clipboard.SetDataObject(obj, copy);
        public static string GetTextFromClipboard() => ClipboardService.GetText() ?? "";
        //public static void SetMinimumWindowSize(GameWindow window, Point size) => System.Windows.Forms.Control.FromHandle(window.Handle).MinimumSize = size.ToSystemSize();
        public static float CaretBlinkTime => 1f; //System.Windows.Forms.SystemInformation.CaretBlinkTime / 500f;
        //public static void SetWindowPosition(DWindow d_window, Point position) => ((System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(d_window.Window.Handle)).Location = new System.Drawing.Point((int)position.X, (int)position.Y);
    }
}
