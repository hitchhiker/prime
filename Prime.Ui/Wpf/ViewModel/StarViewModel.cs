using GalaSoft.MvvmLight.Command;
using Prime.Core;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class StarViewModel
    {
        //public StarViewModel()
        //{
        //    _context = UserContext.Current;

        //    _messenger.Register<BookmarkHasChangedMessage>(this, _context.Token, UpdateStar);
        //    _messenger.Register<BookmarkStatusResponseMessage>(this, _context.Token, UpdateStar);

        //    _messenger.Send<BookmarkStatusRequestMessage>(new BookmarkStatusRequestMessage(vm.GetPageCommand()), _context.Token);

        //    ClickBookmarkCommand = new RelayCommand(() =>
        //    {
        //           _messenger.Send(new BookmarkSetMessage(vm.GetPageCommand(), !isBookmarkSet), _context.Token);
        //    });
        //}

        //private bool isBookmarkSet;
        //private readonly UserContext _context;
        //public readonly Dispatcher Dispatcher;
        //private readonly IMessenger _messenger = DefaultMessenger.I.Default;

        //public RelayCommand ClickBookmarkCommand { get; }

        //private void UpdateStar(BookmarkMessageBase m)
        //{
        //    if (!m.Bookmark.Equals(vm.GetPageCommand()))
        //       return;

        //    Dispatcher.Invoke(() =>
        //    {
        //        isBookmarkSet = m.IsBookmarked;
        //    });
        //}
    }
}
