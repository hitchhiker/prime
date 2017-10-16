using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Core
{
    public class SimpleContentCommand : CommandContent, IEquatable<SimpleContentCommand>
    {
        public SimpleContentCommand() { }

        public SimpleContentCommand(string command)
        {
            command = command.ToLower().Trim();

            var c = Commands.Contains(command);
            if (c)
                Command = command;
        }

        public override int Priority => 0;

        private static readonly UniqueList<string> Commands = new UniqueList<string>()
        {
            "exchange rates",
            "portfolio",
            "watchlist",
            "buy sell",
            "services",
            "wallet",
            "receive",
            "send",
            "coins",
            "exchanges",
            "markets discovery"
        };

        public override CommandBase Parse(string scmd)
        {
            if (Commands.Contains(scmd))
                return new SimpleContentCommand(scmd);

            if (scmd.Length <= 3)
                return null;

            var cmd = Commands.FirstOrDefault(x => x.StartsWith(scmd));
            return !string.IsNullOrWhiteSpace(cmd) ? new SimpleContentCommand(cmd) : null;
        }

        public override string DefaultTitle => Command?.Capitalise();

        public bool Equals(SimpleContentCommand other)
        {
            return base.Equals(this);
        }
    }
}