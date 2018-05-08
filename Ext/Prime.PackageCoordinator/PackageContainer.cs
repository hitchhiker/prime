using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using Prime.Base;
using Prime.Core;
using SharpCompress.Common;
using SharpCompress.Writers;

namespace Prime.Core
{
    public class PackageContainer : UniqueList<PackageInstance>, IEquatable<PackageContainer>
    {
        public readonly Packages Packages;
        private readonly DirectoryInfo _dir;
        public ObjectId Id { get; private set; }
        public PackageCoordinator PackageCoordinator => Packages.Coordinator;

        public PackageContainer(Packages packages, DirectoryInfo dir)
        {
            Packages = packages;
            _dir = dir;
            FindInstances();
        }

        public bool IsEmpty => this.Count == 0;

        private void FindInstances()
        {
            if (_dir != null)
                PopulateFrom(_dir);
        }

        private void PopulateFrom(DirectoryInfo dir)
        {
            foreach (var d in dir.GetDirectories("*", System.IO.SearchOption.TopDirectoryOnly))
            {
                var pi = PackageInstance.Inspect(this, d);
                if (pi == null)
                    continue;

                Add(pi);
                Id = pi.PackageId;
            }
        }

        public bool Equals(PackageContainer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PackageContainer) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return _dir.Name;
        }

        public void Install()
        {
            var i = this[0];
            i.Install();
        }
    }
}