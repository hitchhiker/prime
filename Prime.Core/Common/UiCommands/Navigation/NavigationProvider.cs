using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common.Messages;
using Prime.Utility;

namespace Prime.Common
{
    public class NavigationProvider : UniqueList<NavigationEndpoint>
    {
        public NavigationProvider()
        {
            CommandManager = new CommandManager();
            DefaultMessenger.I.Default.RegisterAsync<CommandAcceptedMessage>(this, CommandAcceptedMessage);
        }

        private void CommandAcceptedMessage(CommandAcceptedMessage m)
        {
            var ame = m.Command;
            if (ame == null)
                return;
            
            Go(new NavigationEndpoint() {Destination = PageUris.I.GetUri(ame) }, UserContext.Current);
        }

        public readonly CommandManager CommandManager;

        public Action<NavigationEndpoint, UserContext> GoAction { get; set; }

        public void Go(NavigationEndpoint endpoint, UserContext context)
        {
            GoAction?.Invoke(endpoint, context);
        }
    }
}
