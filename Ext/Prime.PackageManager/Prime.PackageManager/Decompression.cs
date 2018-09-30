using System.IO;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace Prime.Core
{
    public class Decompression
    {
        public static void ExtractArchive(FileInfo archiveInfo, string destinationPath)
        {
            using (var archive = ArchiveFactory.Open(archiveInfo.FullName, new ReaderOptions() { LeaveStreamOpen = false, LookForHeader = true }))
            {
                if (archive.TotalSize == 0)
                    return;

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