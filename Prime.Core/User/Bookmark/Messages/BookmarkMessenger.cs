using System;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Core
{
    public class BookmarkMessenger : IUserContextMessenger
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        private readonly UserContext _userContext;
        private readonly BookmarkedCommands _bookmarks;

        private BookmarkMessenger(UserContext userContext)
        {
            _userContext = userContext;
            _bookmarks = _userContext.UserSettings.Bookmarks;

            _messenger.RegisterAsync<BookmarkAllRequestMessage>(this, _userContext.Token, BookmarkAllRequestMessage);
            _messenger.RegisterAsync<BookmarkStatusRequestMessage>(this, _userContext.Token, BookmarkStatusRequestMessage);
            _messenger.RegisterAsync<BookmarkSetMessage>(this, _userContext.Token, BookmarkSetMessage);
        }

        public IUserContextMessenger GetInstance(UserContext context)
        {
            return new BookmarkMessenger(context);
        }

        private void BookmarkAllRequestMessage(BookmarkAllRequestMessage m)
        {
            _messenger.Send(new BookmarkAllResponseMessage(_bookmarks.Items.ToUniqueList()), _userContext.Token);
        }

        private void BookmarkStatusRequestMessage(BookmarkStatusRequestMessage m)
        {
            var state = _bookmarks.Items.Contains(m.Bookmark);
            _messenger.Send(new BookmarkStatusResponseMessage(m.Bookmark, state), _userContext.Token);
        }

        private void BookmarkSetMessage(BookmarkSetMessage m)
        {
            if (m.IsBookmarked)
                _bookmarks.Add(m.Bookmark);
            else
                _bookmarks.Remove(m.Bookmark);

            var sr = _userContext.UserSettings.Save(_userContext);

            var state = _bookmarks.Items.Contains(m.Bookmark);

            if (sr.IsSuccess)
                _messenger.Send(new BookmarkHasChangedMessage(m.Bookmark, state), _userContext.Token);
        }

        public void Dispose()
        {
            _messenger.UnregisterAsync(this);
        }
    }
}