using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Prime.Base;
using Prime.Core;

namespace Prime.ExtensionPackager
{
    public class Package : IEnumerable<FileInfo>
    {
        public readonly string Name;
        private readonly List<FileInfo> _files;
        public readonly FileInfo MetaInfo;
        public readonly Assembly Assembly;
        public readonly PackageMeta PackageMeta;

        public List<FileInfo> StagedFiles { get; private set; }
        public FileInfo StagedMeta { get; private set; }
        public DirectoryInfo StagedRoot { get; private set; }
        public readonly List<PackageAssemblyReference> AssemblyReferences = new List<PackageAssemblyReference>();

        public Package(Assembly assembly, PackageMeta meta, FileInfo metaInfo)
        {
            PackageMeta = meta;
            Name = meta.Title.ToLower();
            _files = new List<FileInfo>();
            MetaInfo = metaInfo;
            Assembly = assembly;
        }

        public void AddStagingRange(IEnumerable<FileInfo> files)
        {
            _files.AddRange(files);
        }

        public void AddStaged(DirectoryInfo stagedRoot, IEnumerable<FileInfo> files, FileInfo metaInfo)
        {
            StagedRoot = stagedRoot;
            StagedFiles = files.AsList();
            StagedMeta = metaInfo;
        }

        public IEnumerator<FileInfo> GetEnumerator()
        {
            return _files.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _files).GetEnumerator();
        }

        public string GetDirectory()
        {
            var fsTitle = PackageMeta.Title.Replace(" ", "-");
            if (PackageMeta is IExtensionPlatform plt)
                if (plt.Platform != Platform.NotSpecified)
                    return (fsTitle + "-" + PackageMeta.Id + Path.DirectorySeparatorChar + PackageMeta.Version + "-" + plt.Platform).ToLower() + Path.DirectorySeparatorChar;

            return (fsTitle + "-" + PackageMeta.Id + Path.DirectorySeparatorChar + PackageMeta.Version).ToLower() + Path.DirectorySeparatorChar;
        }

        public string GetCatName()
        {
            if (PackageMeta is IExtensionPlatform plt)
                return "prime-" + (PackageMeta.Id + "-" + PackageMeta.Version + "-" + plt.Platform).ToLower() + ".json";
            return "prime-" + (PackageMeta.Id + "-" + PackageMeta.Version).ToLower() + ".json";
        }

        public void WriteFile()
        {
            if (MetaInfo.Exists)
                MetaInfo.Delete();

            var json = PackageMeta.ToJsonSimple();
            File.WriteAllText(MetaInfo.FullName, json);
            MetaInfo.Refresh();
        }
    }
}