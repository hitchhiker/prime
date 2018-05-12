using System;
using Prime.Base;

namespace Prime.Core
{
    public class ExtensionInstance : IUniqueIdentifier<ObjectId>, IEquatable<ExtensionInstance>
    {
        public ObjectId Id { get; }

        public Version Version { get; }

        public IExtension Extension { get; }

        public ExtensionInstance(IExtension ext)
        {
            Id = ext.Id;
            Version = ext.Version;
            Extension = ext;
        }

        public ExtensionInstance(ObjectId id)
        {
            Id = id;
        }

        public bool Equals(ExtensionInstance other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Id, other.Id) && Equals(Version, other.Version);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ExtensionInstance) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Id != null ? Id.GetHashCode() : 0) * 397) ^ (Version != null ? Version.GetHashCode() : 0);
            }
        }
    }
}