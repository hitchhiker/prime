using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Prime.Radiant.Components
{
    public class Package : IEnumerable<FileInfo>
    {
        public readonly string Name;
        private readonly List<FileInfo> _files;
        public readonly FileInfo ArchiveInfo;

        public Package()
        {
            _files = new List<FileInfo>();
        }

        public Package(string name, List<FileInfo> files, FileInfo archiveInfo)
        {
            Name = name;
            _files = files;
            ArchiveInfo = archiveInfo;
        }

        public IEnumerator<FileInfo> GetEnumerator()
        {
            return _files.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _files).GetEnumerator();
        }
    }
}