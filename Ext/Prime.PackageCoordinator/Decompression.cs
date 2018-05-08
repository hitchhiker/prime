using System.IO;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace Prime.Core
{
    public class Decompression
    {
        public static void ExtractArchive(FileInfo file, string destinationPath)
        {
            var compressed = ArchiveFactory.Open(file, new ReaderOptions(){});
            if (compressed.TotalSize == 0)
                return;

            foreach (var entry in compressed.Entries)
            {
                if (!entry.IsDirectory)
                {
                    entry.WriteToDirectory(destinationPath, new ExtractionOptions(){ExtractFullPath = true, Overwrite = true, PreserveFileTime = true});
                }
            }
        }
    }
}