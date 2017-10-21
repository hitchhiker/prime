using System.Linq;
using Prime.Utility;

namespace Prime.Common
{
    public class ExchangeCommand : CommandBase
    {
        public ExchangeCommand() { }

        public ExchangeCommand(IExchangeProvider provider)
        {
            Exchange = provider;
        }

        public readonly IExchangeProvider Exchange;

        public override CommandBase Parse(string scmd)
        {
            if (!scmd.StartsWith("exc"))
                return null;

            var commands = GetParts(scmd);
            if (commands.Count != 2)
                return null;

            var exs = Networks.I.ExchangeProviders;
            var m = exs.FirstOrDefault(x => x.Network.NameLowered == commands[1]);
            if (m != null)
                return new ExchangeCommand(m);

            m = exs.OrderBy(x=>x.Network.NameLowered.Length).FirstOrDefault(x => x.Network.NameLowered.StartsWith(commands[1]));
            if (m != null)
                return new ExchangeCommand(m);

            return null;
        }
    }
}