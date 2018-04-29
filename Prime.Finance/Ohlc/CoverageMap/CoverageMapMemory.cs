using System;

namespace Prime.Finance
{
    public class CoverageMapMemory : CoverageMapBase, IEquatable<CoverageMapMemory>
    {
        public bool Equals(CoverageMapMemory other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}