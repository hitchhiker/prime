namespace Prime.Core
{
    public abstract class BookmarkMessageBase
    {
        public readonly CommandContent Bookmark;

        public readonly bool IsBookmarked;

        protected BookmarkMessageBase(CommandContent bookmark, bool isBookmarked)
        {
            Bookmark = bookmark;
            IsBookmarked = isBookmarked;
        }
    }
}