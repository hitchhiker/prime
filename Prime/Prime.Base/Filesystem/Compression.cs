using System.Collections.Generic;
using System.IO;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace Prime.Base
{
    public class Compression
    {
        public static CompressionType CompressionType = CompressionType.BZip2;
        public static ArchiveType ArchiveType = ArchiveType.Zip;

        public static FileInfo CreateArchive(DirectoryInfo root, IEnumerable<FileInfo> files, string destinationPath)
        {
            var vpOffset = root.FullName.Length;

            using (Stream stream = File.OpenWrite(destinationPath))
            using (var writer = WriterFactory.Open(stream, ArchiveType, new WriterOptions(CompressionType)))
            {
                foreach (var f in files)
                {
                    var vp = f.FullName.Substring(vpOffset);

                    if (vp[0]==Path.DirectorySeparatorChar)
                        vp = vp.Substring(1);

                    writer.Write(vp, f);
                }
            }

            return new FileInfo(destinationPath);
        }

        public static void ExctractArchive(FileInfo archiveInfo, string destinationPath)
        {
            using (var archive = ArchiveFactory.Open(archiveInfo.FullName, new ReaderOptions() { LeaveStreamOpen = false, LookForHeader = true }))
            {
                var reader = archive.ExtractAllEntries();
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                        reader.WriteEntryToDirectory(destinationPath, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                }
            }
        }
    }
}