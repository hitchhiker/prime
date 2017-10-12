using System;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Core
{
    public class BookmarkController
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        private readonly UserContext _userContext;

        internal BookmarkController(UserContext userContext)
        {
            _userContext = userContext;

            var bookmarks = _userContext.UserSettings.BookmarkedCommands;

            _messenger.Register<BookmarkAllRequestMessage>(this, _userContext.Token, m =>
            {
                _messenger.Send(new BookmarkAllResponseMessage(bookmarks.Items.ToUniqueList()), this);
            });

            _messenger.Register<BookmarkStatusRequestMessage>(this, _userContext.Token, m =>
            {
                var bmd = bookmarks.Items.Contains(m.Bookmark);
                _messenger.Send(new BookmarkStatusResponseMessage(m.Bookmark, bmd), this);
            });

            _messenger.Register<BookmarkSetMessage>(this, _userContext.Token, m =>
            {
                if (m.IsBookmarked)
                    bookmarks.Add(m.Bookmark);
                else
                    bookmarks.Remove(m.Bookmark);

                var sr = _userContext.UserSettings.Save(_userContext);

                if (sr.IsSuccess)
                    _messenger.Send(new BookmarkHasChangedMessage(m.Bookmark, m.IsBookmarked), _userContext.Token);
            });
        }
    }
}