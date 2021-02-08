namespace DownUnder.UI.Utilities.CommonNamespace
{
    public interface IOSInterface
    {
        void CopyTextToClipBoard(string text);
        string GetTextFromClipboard();
        float CaretBlinkTime { get; }
        string ReadAllText(string path);
        void WriteAllText(string path, string contents);
    }
}
