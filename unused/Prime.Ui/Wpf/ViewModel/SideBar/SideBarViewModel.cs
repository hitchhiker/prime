using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Prime.Common;
using Prime.Ui.Wpf.ViewModel;
using Prime.Ui.Wpf.Components;

namespace Prime.Ui.Wpf.ViewModel
{
    public class SideBarViewModel : VmBase
    {
        private readonly ScreenViewModel _screenVm;
        private readonly NavigationProvider _nav;

        public SideBarViewModel()
        {
        }

        public SideBarViewModel(ScreenViewModel screenVm) : this()
        {
            _screenVm = screenVm;
            _nav = screenVm.NavigationProvider;

            _sideBarWidth = 40;
            MenuClickedCommand = new RelayCommand(ToggleMenu);
            BookmarkClickedCommand = new RelayCommand(delegate ()
            {
                IsBookmarksOpen = !IsBookmarksOpen;
                ClickMenuItem<BookmarksSideBarViewModel>();
            });

            PortfolioClickedCommand = new RelayCommand(() => { _nav.IssueCommand("portfolio"); });
            SettingsClickedCommand = new RelayCommand(() => { _nav.IssueCommand("services"); });
            BuySellClickedCommand = new RelayCommand(() => { _nav.IssueCommand("buy sell"); });
            WatchlistClickedCommand = new RelayCommand(() => { _nav.IssueCommand("watchlist"); });
            ExchangesClickedCommand = new RelayCommand(() => { _nav.IssueCommand("exchanges"); });
            CoinsClickedCommand = new RelayCommand(() => { _nav.IssueCommand("coins"); });
            MarketsDiscoveryClickedCommand = new RelayCommand(() => { _nav.IssueCommand("markets discovery"); });
            ExchangeRatesClickedCommand = new RelayCommand(() => { _nav.IssueCommand("exchange rates"); });
            ReceiveClickedCommand = new RelayCommand(() => { _nav.IssueCommand("receive"); });
            DataExplorerClickedCommand = new RelayCommand(() => { _nav.IssueCommand("data explorer"); });
        }

        public RelayCommand MenuClickedCommand { get; private set; }
        public RelayCommand BookmarkClickedCommand { get; private set; }
        public RelayCommand PortfolioClickedCommand { get; private set; }
        public RelayCommand SettingsClickedCommand { get; private set; }
        public RelayCommand BuySellClickedCommand { get; private set; }
        public RelayCommand WatchlistClickedCommand { get; private set; }
        public RelayCommand ExchangesClickedCommand { get; private set; }
        public RelayCommand CoinsClickedCommand { get; private set; }
        public RelayCommand MarketsDiscoveryClickedCommand { get; private set; }
        public RelayCommand ExchangeRatesClickedCommand { get; private set; }
        public RelayCommand ReceiveClickedCommand { get; private set; }
        public RelayCommand DataExplorerClickedCommand { get; private set; }

        public bool IsDemoVisible => PrimeWpf.I.IsDemo;

        private bool _isMenuOpen;
        public bool IsMenuOpen
        {
            get => _isMenuOpen;
            set => SetAfter(ref _isMenuOpen, value, OpenChanged);
        }

        private bool _isBookmarksOpen;
        public bool IsBookmarksOpen
        {
            get => _isBookmarksOpen;
            set => Set(ref _isBookmarksOpen, value);
        }

        private bool _isSettingsOpen;
        public bool IsSettingsOpen
        {
            get => _isSettingsOpen;
            set => Set(ref _isSettingsOpen, value);
        }

        private bool _isPortfolioOpen;
        public bool IsPortfolioOpen
        {
            get => _isPortfolioOpen;
            set => Set(ref _isPortfolioOpen, value);
        }

        private int _sideBarWidth;
        public int SideBarWidth
        {
            get => _sideBarWidth;
            set => Set(ref _sideBarWidth, value);
        }

        private VmBase _currentExpandedModel;
        public VmBase CurrentExpandedModel
        {
            get => _currentExpandedModel;
            set => Set(ref _currentExpandedModel, value);
        }

        private void OpenChanged(bool newState)
        {
            SideBarWidth = newState ? 200 : 40;
        }

        private void ToggleMenu()
        {
            IsMenuOpen = !_isMenuOpen;
        }

        private readonly ConcurrentDictionary<Type, VmBase> _usedModels = new ConcurrentDictionary<Type, VmBase>();

        public void ClickMenuItem<T>() where T : PanelFlyoutViewModelBase, new()
        {
            if (IsMenuOpen)
            {
                ToggleMenu();
                return;
            }

            CurrentExpandedModel = _usedModels.GetOrAdd(typeof(T), t => new T { ScreenViewModel = _screenVm });
            IsMenuOpen = true;
        }
    }
}
