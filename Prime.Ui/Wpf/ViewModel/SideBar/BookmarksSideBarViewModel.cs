using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Prime.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Prime.Ui.Wpf.ViewModel;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class BookmarksSideBarViewModel : PanelFlyoutViewModelBase
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;

        public BookmarksSideBarViewModel()
        {
            ItemClickedCommand = new RelayCommand<CommandContent>(Clicked);

            _messenger.RegisterAsync<BookmarkAllResponseMessage>(this, UserContext.Current.Token, BookmarkAllResponseMessage);
            _messenger.RegisterAsync<BookmarkHasChangedMessage>(this, UserContext.Current.Token, BookmarkHasChangedMessage);
            _messenger.SendAsync(new BookmarkAllRequestMessage(), UserContext.Current.Token);
        }

        private void BookmarkHasChangedMessage(BookmarkHasChangedMessage m)
        {
            var state = _bookmarks.Contains(m.Bookmark);
            UiDispatcher.Invoke(() =>
            {
                if (m.IsBookmarked && state || !m.IsBookmarked && !state)
                    return;

                if (m.IsBookmarked)
                    _bookmarks.Add(m.Bookmark);
                else
                    _bookmarks.Remove(m.Bookmark);
            });
        }

        private void BookmarkAllResponseMessage(BookmarkAllResponseMessage m)
        {
            UiDispatcher.Invoke(() =>
            {
                _bookmarks.Clear();
                foreach (var i in m.Bookmarks)
                    _bookmarks.Add(i);
            });
        }

        public RelayCommand<CommandContent> ItemClickedCommand { get; private set; }

        //...
        private BindingList<CommandContent> _bookmarks = new BindingList<CommandContent>();

        /// <summary>
        /// A log of a starting process
        /// </summary>
        public BindingList<CommandContent> Bookmarks
        {
            get => _bookmarks;
            set => Set(ref _bookmarks, value);
        }

        private void Clicked(CommandContent sender)
        {
            ScreenViewModel.CommandManager.IssueCommand(UserContext.Current, sender);
        }
    }
}