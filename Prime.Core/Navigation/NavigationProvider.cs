using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Core
{
    public class NavigationProvider : UniqueList<NavigationEndpoint>
    {
        public NavigationProvider()
        {
            CommandManager = new CommandManager();
            CommandManager.CommandAccepted.SubscribePermanent(OnCommandAccepted);
        }

        public readonly CommandManager CommandManager;

        public Action<NavigationEndpoint, UserContext> GoAction { get; set; }

        public void Go(NavigationEndpoint endpoint, UserContext context)
        {
            GoAction?.Invoke(endpoint, context);
        }

        private void OnCommandAccepted(CommandManagerEvent e)
        {
            var ame = e.Command as CommandContent;
            if (ame == null)
                return;

           // Go(new NavigationEndpoint() {Destination = PageUris.I.GetUri(ame) }, UserContext.Current);
        }
    }
}
