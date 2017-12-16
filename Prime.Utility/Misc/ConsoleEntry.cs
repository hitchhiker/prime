namespace Prime.Utility
{
    public class ConsoleEntry
    {
        public ConsoleEntry(string text)
        {
            ColumnWidth = -1;
            Text = text;
        }

        public ConsoleEntry(int width, string text) : this(text)
        {
            ColumnWidth = width;
            if (text.Length > width && width>2)
                Text = text.Substring(0, width - 2) + "~";
        }

        public int ColumnWidth;
        public string Text;
    }
}