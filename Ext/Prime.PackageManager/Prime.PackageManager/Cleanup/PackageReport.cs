using Prime.Base;

namespace Prime.Core
{
    public class PackageReport
    {
        public UniqueList<ObjectId> Required { get; set; } = new UniqueList<ObjectId>();
        public UniqueList<PackageInstance> Distributed { get; set; } = new UniqueList<PackageInstance>();
        public UniqueList<PackageInstance> Installed { get; set; } = new UniqueList<PackageInstance>();
        public UniqueList<ObjectId> RequiresInstallOnly { get; set; } = new UniqueList<ObjectId>();
        public UniqueList<ObjectId> RequiresDistribution { get; set; } = new UniqueList<ObjectId>();
    }
}