using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Prime.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Prime.Ui.Wpf.ViewModel;

namespace Prime.Ui.Wpf.ViewModel
{
    public class BookmarksSideBarViewModel : PanelFlyoutViewModelBase
    {
        public BookmarksSideBarViewModel()
        {
            ItemClickedCommand = new RelayCommand<CommandContent>(Clicked);
        }

        public RelayCommand<CommandContent> ItemClickedCommand { get; private set; }

        //...
        private ObservableCollection<CommandContent> _bookmarks = UserContext.Current.UserSettings.BookmarkedCommands.InitObservable();

        /// <summary>
        /// A log of a starting process
        /// </summary>
        public ObservableCollection<CommandContent> Bookmarks
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