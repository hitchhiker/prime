using Prime.Common;

namespace Prime.Common
{
    public class BookmarkAllResponseMessage
    {
        public readonly UniqueList<CommandContent> Bookmarks;

        public BookmarkAllResponseMessage(UniqueList<CommandContent> bookmarks)
        {
            Bookmarks = bookmarks;
        }    
    }
}