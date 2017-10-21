namespace Prime.Common
{
    public class BookmarkStatusRequestMessage
    {
        public readonly CommandContent Bookmark;

        public BookmarkStatusRequestMessage(CommandContent bookmark)
        {
            Bookmark = bookmark;
        }
    }
}