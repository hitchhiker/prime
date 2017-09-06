using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Core
{
    public class CommandManager
    {
        private IReadOnlyList<CommandBase> _providers;
        public IReadOnlyList<CommandBase> Providers => _providers ?? (_providers = TypeCatalogue.I.ImplementInstances<CommandBase>().OrderByDescending(x => x.Priority).ToList());

        public readonly ObservableEvent<CommandManagerEvent, CommandBase> CommandAccepted =
            new ObservableEvent<CommandManagerEvent,  CommandBase>(command => new CommandManagerEvent(command));

        public CommandBase Parse(string cstr)
        {
            if (string.IsNullOrWhiteSpace(cstr))
                return null;

            cstr = cstr.ToLower().Trim();
            
            var cm = Providers.Select(x=>x.Parse(cstr)).FirstOrDefault(x => x!=null);
            if (cm != null)
                return cm;

            //try shorter cuts
            cm = Providers.Select(x => x.Parse("a " + cstr)).FirstOrDefault(x => x != null);
            
            return cm;
        }

        public void IssueCommand(object source, UserContext ctx, string cstr)
        {
            var c = Parse(cstr);
            if (c == null)
                return;

            IssueCommand(ctx, c);
        }

        public void IssueCommand(UserContext ctx, CommandBase command)
        {
            ctx.LastCommand = command;
            CommandAccepted.Publish(new CommandManagerEvent(command));
        }
    } 
}
