using System;

namespace Prime.Core
{
    public class CommandContent : IEquatable<CommandContent>
    {
        public bool Equals(CommandContent other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CommandContent) obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}