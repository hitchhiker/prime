using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;

namespace Prime.Ui.Wpf.ViewModel
{
    public class AddressBoxModel : VmBase
    {
        private readonly AddressBarModel _addressBarModel;
        private readonly ScreenViewModel _screenViewModel;
        private readonly CommandManager _commandManager;

        public AddressBoxModel() : base() { }

        public AddressBoxModel(IMessenger messenger, ScreenViewModel screenViewModel, AddressBarModel addressBarModel) : base(messenger)
        {
            _addressBarModel = addressBarModel;
            _screenViewModel = screenViewModel;
            OnEnterCommand = new RelayCommand<string>(IssueCommandStart);
            
            _commandManager = _screenViewModel.CommandManager;
            _commandManager.CommandAccepted.SubscribePermanent(e => { LastCommandAccepted = e; });
        }

        public RelayCommand<string> OnEnterCommand { get; set; }

        private void IssueCommandStart(string enteredText)
        {
            _commandManager.IssueCommand(this, UserContext.Current, enteredText);
            CommandAttemptedText = enteredText;
        }
        
        private CommandManagerEvent _lastCommandAccepted;
        public CommandManagerEvent LastCommandAccepted
        {
            get => _lastCommandAccepted;
            set => Set(ref _lastCommandAccepted, value);
        }

        private string _commandAttemptedText;
        public string CommandAttemptedText
        {
            get => _commandAttemptedText;
            set => Set(ref _commandAttemptedText, value);
        }
    }
}
