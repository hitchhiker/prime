using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Prime.Common;
using Prime.Common.Messages;
using Prime.Ui.Wpf.Components;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class AddressBoxModel : VmBase, IDisposable
    {
        private readonly AddressBarModel _addressBarModel;
        private readonly ScreenViewModel _screenViewModel;

        public AddressBoxModel() : base() { }

        public AddressBoxModel(ScreenViewModel screenViewModel, AddressBarModel addressBarModel)
        {
            _addressBarModel = addressBarModel;
            _screenViewModel = screenViewModel;
            OnEnterCommand = new RelayCommand<string>(IssueCommandStart);

            DefaultMessenger.I.Default.RegisterAsync<CommandAcceptedMessage>(this, _screenViewModel.Id, CommandAcceptedMessage);
        }

        private void CommandAcceptedMessage(CommandAcceptedMessage m)
        {
            if (m.Command!=null)
                LastCommandAccepted = m.Command;
        }

        public RelayCommand<string> OnEnterCommand { get; set; }

        private void IssueCommandStart(string enteredText)
        {
            _screenViewModel.NavigationProvider.IssueCommand(enteredText);
            CommandAttemptedText = enteredText;
        }
        
        private CommandBase _lastCommandAccepted;
        public CommandBase LastCommandAccepted
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

        public void Dispose()
        {
            DefaultMessenger.I.Default.UnregisterD(this);
        }
    }
}
