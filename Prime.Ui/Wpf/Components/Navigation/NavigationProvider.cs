using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Messages;
using Prime.Ui.Wpf.ViewModel;
using Prime.Utility;

namespace Prime.Ui.Wpf.Components
{
    public class NavigationProvider : IDisposable
    {
        private readonly ScreenViewModel _model;
        private readonly Action<CommandBase> _onAccepted;

        public NavigationProvider(ScreenViewModel model, Action<CommandBase> onAccepted)
        {
            _model = model;
            _onAccepted = onAccepted;
            DefaultMessenger.I.Default.RegisterAsync<CommandAcceptedMessage>(this, _model.Id, CommandAcceptedMessage);
        }

        private void CommandAcceptedMessage(CommandAcceptedMessage m)
        {
            _model.UiDispatcher.Invoke(() =>
            {
                _onAccepted.Invoke(m.Command);
            });
        }

        public void IssueCommand(string command)
        {
            DefaultMessenger.I.Default.SendAsync(new CommandIssuedMessage(_model.Id, command));
        }

        public void Dispose()
        {
            DefaultMessenger.I.Default.UnregisterD(this);
        }
    }
}
