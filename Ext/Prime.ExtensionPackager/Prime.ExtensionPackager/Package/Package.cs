using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prime.Core;

namespace Prime.ExtensionPackager
{
    public class Package : IEnumerable<FileInfo>
    {
        public readonly string Name;
        private readonly List<FileInfo> _files;
        public readonly FileInfo MetaInfo;
        public readonly IExtension Extension;

        public Package(IExtension ext, FileInfo metaInfo)
        {
            Extension = ext;
            Name = ext.Title.ToLower();
            _files = new List<FileInfo>();
            MetaInfo = metaInfo;
        }

        public Package(IExtension ext, List<FileInfo> files, FileInfo metaInfo)
        {
            Extension = ext;
            Name = ext.Title.ToLower();
            _files = files;
            MetaInfo = metaInfo;
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
            return (Extension.Title + "-" + Extension.Id.ToString() + Path.DirectorySeparatorChar +
                    Extension.Version + "-" + Extension.Platform + Path.DirectorySeparatorChar).ToLower();
        }

        public List<FileInfo> StagedFiles { get; private set; }
        public FileInfo StagedMeta { get; private set; }
        public DirectoryInfo StagedRoot { get; private set; }
    }
}