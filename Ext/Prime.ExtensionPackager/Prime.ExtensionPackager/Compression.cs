using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Writers;

namespace Prime.ExtensionPackager
{
    public class Compression
    {
        public static CompressionType CompressionType = CompressionType.BZip2;
        public static ArchiveType ArchiveType = ArchiveType.Zip;

        public static void CreateArchive(DirectoryInfo root, IEnumerable<FileInfo> files, string destinationPath)
        {
            var vpOffset = root.FullName.Length;

            using (Stream stream = File.OpenWrite(destinationPath))
            using (var writer = WriterFactory.Open(stream, ArchiveType, new WriterOptions(CompressionType)))
            {
                foreach (var f in files)
                {
                    var vp = f.FullName.Substring(vpOffset);

                    if (vp.StartsWith(Path.DirectorySeparatorChar))
                        vp = vp.Substring(1);

                    writer.Write(vp, f);
                }
            }
        }
    }
}