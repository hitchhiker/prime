using System;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Core
{
    public class BookmarkMessenger
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        private readonly UserContext _userContext;
        private readonly BookmarkedCommands _bookmarks;

        internal BookmarkMessenger(UserContext userContext)
        {
            _userContext = userContext;
            _bookmarks = _userContext.UserSettings.BookmarkedCommands;

            _messenger.Register<BookmarkAllRequestMessage>(this, _userContext.Token, BookmarkAllRequestMessage);
            _messenger.Register<BookmarkStatusRequestMessage>(this, _userContext.Token, BookmarkStatusRequestMessage);
            _messenger.Register<BookmarkSetMessage>(this, _userContext.Token, BookmarkSetMessage);
        }

        private void BookmarkAllRequestMessage(BookmarkAllRequestMessage m)
        {
            _messenger.Send(new BookmarkAllResponseMessage(_bookmarks.Items.ToUniqueList()), _userContext.Token);
        }

        private void BookmarkStatusRequestMessage(BookmarkStatusRequestMessage m)
        {
            var bmd = _bookmarks.Items.Contains(m.Bookmark);
            _messenger.Send(new BookmarkStatusResponseMessage(m.Bookmark, bmd), _userContext.Token);
        }

        private void BookmarkSetMessage(BookmarkSetMessage m)
        {
            if (m.IsBookmarked)
                _bookmarks.Add(m.Bookmark);
            else
                _bookmarks.Remove(m.Bookmark);

            var sr = _userContext.UserSettings.Save(_userContext);

            if (sr.IsSuccess)
                _messenger.Send(new BookmarkHasChangedMessage(m.Bookmark, m.IsBookmarked), _userContext.Token);
        }
    }
}