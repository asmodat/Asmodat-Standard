namespace AsmodatStandard.IO
{
    public static class ClipboardHelper
    {
        public static string GetText() => TextCopy.Clipboard.GetText();
        public static void SetText(string text) => TextCopy.Clipboard.SetText(text);

    }
}