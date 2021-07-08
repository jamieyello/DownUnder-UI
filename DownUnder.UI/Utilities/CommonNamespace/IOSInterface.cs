namespace DownUnder.UI
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
