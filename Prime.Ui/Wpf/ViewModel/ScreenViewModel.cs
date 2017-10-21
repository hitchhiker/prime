using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Prime.Common;
using Prime.Ui.Wpf.Components;
using Xceed.Wpf.AvalonDock;

namespace Prime.Ui.Wpf.ViewModel
{
    public class ScreenViewModel : VmBase, IDisposable
    {
        private readonly ILayoutManager _layoutManager;
        private readonly DockingManager _dockingManager;
        private readonly IMessenger _messenger;

        private ObservableCollection<DocumentPaneViewModel> _documents = new ObservableCollection<DocumentPaneViewModel>();

        public RelayCommand TestCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }

        public RelayCommand SettingsCommand { get; }
        public RelayCommand WalletReceiveCommand { get; }
        public RelayCommand WalletSendCommand { get; }

        public RelayCommand<DockingManager> SaveLayoutCommand { get; }
        public RelayCommand<DockingManager> ResetLayoutCommand { get; }
        public RelayCommand<DockingManager> RestoreLayoutCommand { get; }
        public readonly NavigationProvider NavigationProvider;

        public string Title { get; } = "prime [pre-alpha v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + "]";

        public ObservableCollection<DocumentPaneViewModel> Documents
        {
            get => _documents;
            set => Set(ref _documents, value);
        }

        public void AddDocument(DocumentPaneViewModel viewModel)
        {
            Documents.Add(viewModel);
            RaisePropertyChanged(nameof(Documents));
        }

        public void RemoveDocument(DocumentPaneViewModel viewModel)
        {
            Documents.Add(viewModel);
            RaisePropertyChanged(nameof(Documents));
        }

        public IEnumerable<ToolPaneViewModel> Tools
        {
            get
            {
                yield return LogPane;
            }
        }

        public LogPanelViewModel LogPane { get; set; }

        public AddressBarModel AddressBarModel { get; }

        public SideBarViewModel SideBarViewModel { get; }

        public ScreenViewModel(IMessenger messenger, ILayoutManager layoutManager, DockingManager dockingManager)
        {
            dockingManager.DocumentClosed += DockingManager_DocumentClosed;

            NavigationProvider = new NavigationProvider(this, OnCommandAccepted);

            AddressBarModel = new AddressBarModel(messenger, this);
            SideBarViewModel = new SideBarViewModel(this);

            _layoutManager = layoutManager;
            _dockingManager = dockingManager;
            _messenger = messenger;

            LogPane = new LogPanelViewModel(messenger);

            ExitCommand = new RelayCommand(() => Application.Current.Shutdown());

            SaveLayoutCommand = new RelayCommand<DockingManager>(manager => _layoutManager.SaveLayout(manager));
            RestoreLayoutCommand = new RelayCommand<DockingManager>(manager => _layoutManager.LoadLayout(manager));
            ResetLayoutCommand = new RelayCommand<DockingManager>(manager => _layoutManager.ResetLayout(manager));
            SettingsCommand = new RelayCommand(() => { SideBarViewModel.SettingsClickedCommand.Execute(null); });
        }

        private void OnCommandAccepted(CommandBase c)
        {
            var inst = PrimeWpf.I.PanelProviders.FirstOrDefault(x => x.IsFor(c))?.GetInstance(_messenger, this, c);
            if (inst == null)
                return;

            UiDispatcher.Invoke(() =>
            {
                AddDocument(inst);
            });
        }

        private void DockingManager_DocumentClosed(object sender, DocumentClosedEventArgs e)
        {
            if (!(e.Document?.Content is DocumentPaneViewModel doc))
                return;

            if (Documents.Contains(doc))
                Documents.Remove(doc);
        }

        public void Dispose()
        {
            NavigationProvider?.Dispose();
            _dockingManager.DocumentClosed -= DockingManager_DocumentClosed;
        }
    }
}