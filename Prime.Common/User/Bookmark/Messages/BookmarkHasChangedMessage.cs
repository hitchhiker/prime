namespace Prime.Common
{
    public class BookmarkHasChangedMessage : BookmarkMessageBase
    {
        public BookmarkHasChangedMessage(CommandContent bookmark, bool isBookmarked) : base(bookmark, isBookmarked)
        {
        }
    }
}