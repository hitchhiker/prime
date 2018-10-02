using System;

namespace Prime.Base
{
    public class CommandArg : IEquatable<CommandArg>
    {
        public readonly string Verb;
        public readonly string Help;

        public readonly Func<string[], bool> Func;

        public CommandArg(string verb, string help, Func<string[], bool> func)
        {
            Verb = verb;
            Func = func;
            Help = help;
        }

        public CommandArg(string verb, string help, Action<string[]> parser)
        {
            Verb = verb;
            Func = args =>
            {
                args = CommandArgs.RemoveItem(args, verb);
                parser(args);
                return true;
            };

            Help = help;
        }

        public bool Equals(CommandArg other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Verb, other.Verb);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CommandArg) obj);
        }

        public override int GetHashCode()
        {
            return (Verb != null ? Verb.GetHashCode() : 0);
        }
    }
}