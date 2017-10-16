namespace Prime.Core
{
    public class BookmarkSetMessage : BookmarkMessageBase
    {
        public BookmarkSetMessage(CommandContent bookmark, bool isBookmarked) : base(bookmark, isBookmarked)
        {
        }
    }
}