namespace Prime.Core
{
    public class BookmarkSetMessage : BookmarkMessageBase
    {
        protected BookmarkSetMessage(CommandContent bookmark, bool isBookmarked) : base(bookmark, isBookmarked)
        {
        }
    }
}