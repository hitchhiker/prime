using System;
using System.Collections.Generic;
using Prime.Utility;

namespace Prime.Common
{
    public abstract class CommandBase : ModelBase, IEquatable<CommandBase>
    {
        protected CommandBase() { }

        public abstract CommandBase Parse(string scmd);

        /// <summary>
        /// 1 = Top Priority
        /// 100 = Lowest Priority
        /// </summary>
        /// 
        public virtual int Priority => 50;

        [Bson]
        public string Command { get; protected set; }

        protected List<string> GetParts(string scmd)
        {
            return scmd.ToCsv(true, ' ');
        }

        public bool Equals(CommandBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Command, other.Command);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CommandBase) obj);
        }

        public override int GetHashCode()
        {
            return (Command != null ? Command.GetHashCode() : 0);
        }
    }
}