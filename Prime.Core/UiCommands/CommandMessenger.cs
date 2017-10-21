using System;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Prime.Common;
using Prime.Common.Messages;
using Prime.Utility;

namespace Prime.Core
{
    internal class CommandMessenger : IStartupMessenger
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        private readonly CommandProvider _provider = new CommandProvider();

        internal CommandMessenger()
        {
            _messenger.RegisterAsync<CommandIssuedMessage>(this, CommandIssuedMessage);
        }

        private void CommandIssuedMessage(CommandIssuedMessage m)
        {
            var c = _provider.Parse(m.StringCommand);
            if (c == null)
                return;

            _messenger.SendAsync(new CommandAcceptedMessage(c), m.ContainerId);
        }
    }
}
