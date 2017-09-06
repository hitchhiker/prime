using System;

namespace Prime.Core
{
    public class NavigationEndpoint : IEquatable<NavigationEndpoint>
    {
        public string Title { get; set; }

        public bool FullPage { get; set; }

        public string Destination { get; set; }

        public object DataContext { get; set; }

        public bool Equals(NavigationEndpoint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Destination, other.Destination);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NavigationEndpoint) obj);
        }

        public override int GetHashCode()
        {
            return (Destination != null ? Destination.GetHashCode() : 0);
        }
    }
}