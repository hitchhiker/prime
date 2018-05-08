using System;
using System.IO;
using System.Linq;
using LiteDB;
using Prime.Base;
using Prime.Core;

namespace Prime.Core
{
    public class PackageInstance : IEquatable<PackageInstance>
    {
        public readonly PackageContainer Package;

        public readonly DirectoryInfo Directory;

        public readonly ObjectId PackageId;

        public readonly PackageMeta MetaInfo;

        public readonly Platform Platform;

        public readonly Version Version;

        public readonly string Title;

        private readonly string _key;

        private PackageInstance(PackageContainer package, DirectoryInfo directory, PackageMeta metaInfo)
        {
            Package = package;
            Directory = directory;
            PackageId = metaInfo.Id;
            MetaInfo = metaInfo;
            Platform = metaInfo.Platform;
            Version = metaInfo.Version;
            Title = metaInfo.Title;
            _key = PackageId + ":" + Platform + ":" + Version;
        }

        public static PackageInstance Inspect(PackageContainer container, DirectoryInfo directory)
        {
            var pm = directory.GetFiles(PackageCoordinator.PrimeExtName);
            if (pm.Length != 1)
                return null;

            var mi = PackageMeta.From(pm[0]);
            if (mi == null)
                return null;

            return new PackageInstance(container, directory, mi);
        }

        public bool Equals(PackageInstance other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_key, other._key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PackageInstance) obj);
        }

        public override int GetHashCode()
        {
            return (_key != null ? _key.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return Title + " " + Version + " " + Platform;
        }

        public void Install()
        {
            var pc = Package.PackageCoordinator.Context;
            var dest = Path.Combine(pc.FileSystem.InstallDirectory.FullName, GetDirectory());

            var files = Directory.GetFiles("*", SearchOption.AllDirectories).ToList();
            var arcs = files.Where(x => x.Name.StartsWith(PackageCoordinator.ArchiveName)).ToList();
            var store = files.Except(arcs).ToList();

            foreach (var arc in arcs)
                Decompression.ExtractArchive(arc, dest);

            foreach (var f in store)
                f.CopyTo(Path.Combine(dest, f.FullName.Substring(Directory.FullName.Length + 1)));
        }

        public string GetDirectory()
        {
            var fsTitle = Title.Replace(" ", "-");
            if (Platform != Platform.NotSpecified)
                    return (fsTitle + "-" + PackageId + Path.DirectorySeparatorChar + Version + "-" + Platform).ToLower() + Path.DirectorySeparatorChar;

            return (fsTitle + "-" + PackageId + Path.DirectorySeparatorChar + Version).ToLower() + Path.DirectorySeparatorChar;
        }
    }
}