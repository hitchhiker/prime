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

            _messenger.Register<BookmarkAllResponseMessage>(this, UserContext.Current.Token, m =>
            {
                _bookmarks.Clear();
                foreach (var i in m.Bookmarks)
                    _bookmarks.Add(i);
            });

            _messenger.Send(new BookmarkAllRequestMessage(), UserContext.Current.Token);
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