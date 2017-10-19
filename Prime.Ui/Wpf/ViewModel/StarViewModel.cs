using System;
using GalaSoft.MvvmLight.Command;
using Prime.Core;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class StarViewModel : VmBase, IDisposable
    {
        private readonly DocumentPaneViewModel _paneVm;
        private readonly CommandContent _paneCommand;

        public StarViewModel(DocumentPaneViewModel paneVm)
        {
            _paneVm = paneVm;
            _context = UserContext.Current;
            _paneCommand = _paneVm.GetPageCommand();

            _messenger.RegisterAsync<BookmarkHasChangedMessage>(this, _context.Token, SetBookmarkedState);
            _messenger.RegisterAsync<BookmarkStatusResponseMessage>(this, _context.Token, SetBookmarkedState);

            _messenger.SendAsync<BookmarkStatusRequestMessage>(new BookmarkStatusRequestMessage(_paneCommand), _context.Token);

            ClickBookmarkCommand = new RelayCommand(() =>
            {
                //IsBookmarked responds to clicks, and will be reflect that after-clicked state.
                _messenger.SendAsync(new BookmarkSetMessage(_paneCommand, IsBookmarked), _context.Token);
            });

            IsEnabled = true;
        }

        private bool _isBookmarked;
        public bool IsBookmarked
        {
            get => _isBookmarked;
            set => Set(ref _isBookmarked, value);
        }

        public bool IsEnabled { get; }

        private readonly UserContext _context;

        private readonly IMessenger _messenger = DefaultMessenger.I.Default;

        public RelayCommand ClickBookmarkCommand { get; }

        private void SetBookmarkedState(BookmarkMessageBase m)
        {
            if (!m.Bookmark.Equals(_paneCommand))
               return;

            UiDispatcher.Invoke(() =>
            {
                IsBookmarked = m.IsBookmarked;
            });
        }

        public void Dispose()
        {
            _messenger.UnregisterAsync(this);
        }
    }
}
