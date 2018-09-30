namespace Prime.Core
{
    public class BookmarkStatusResponseMessage : BookmarkMessageBase
    {
        public BookmarkStatusResponseMessage(CommandContent bookmark, bool isBookmarked) : base(bookmark, isBookmarked)
        {
        }
    }
}