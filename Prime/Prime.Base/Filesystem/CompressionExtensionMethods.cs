using System;
using System.Collections.Generic;
using System.IO;
using Prime.Core;

namespace Prime.Base
{
    public static class CompressionExtensionMethods
    {
        public static FileInfo CreateArchive(this DirectoryInfo root, string destination, SearchOption searchOption)
        {
            return Compression.CreateArchive(root, root.EnumerateFiles("*", searchOption), destination);
        }

        public static FileInfo CreateArchive(this DirectoryInfo root, string destination, string wildCard = "*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            return Compression.CreateArchive(root, root.EnumerateFiles(wildCard, searchOption), destination);
        }

        public static FileInfo CreateArchive(this FileInfo file, string destination)
        {
            return Compression.CreateArchive(file.Directory, new List<FileInfo>() {file}, destination);
        }

        public static FileInfo CreateArchive(this DirectoryInfo root, FileInfo file, string archiveName)
        {
            return Compression.CreateArchive(root, new List<FileInfo>() { file }, Path.Combine(root.FullName, archiveName));
        }

        public static FileInfo CreateArchive(this DirectoryInfo root, FileInfo file1, FileInfo file2, string archiveName)
        {
            return Compression.CreateArchive(root, new List<FileInfo>() { file1, file2 }, Path.Combine(root.FullName, archiveName));
        }

        public static FileInfo CreateArchive(this DirectoryInfo root, FileInfo file1, FileInfo file2, FileInfo file3, string archiveName)
        {
            return Compression.CreateArchive(root, new List<FileInfo>() { file1, file2, file3 }, Path.Combine(root.FullName, archiveName));
        }

        public static FileInfo CreateArchive(this DirectoryInfo root, IEnumerable<FileInfo> files, string destination)
        {
            return Compression.CreateArchive(root, files, destination);
        }

        public static void ExtractArchive(this FileInfo archiveInfo, string destinationPath)
        {
            Compression.ExctractArchive(archiveInfo, destinationPath);
        }

        public static DirectoryInfo ExtractArchive(this FileInfo archiveInfo, PrimeContext context)
        {
            var tmpDir = context.FileSystem.GetTmpSubDirectory("tmp-extract", ObjectId.NewObjectId().ToString());
            Compression.ExctractArchive(archiveInfo, tmpDir.FullName);
            return tmpDir;
        }
    }
}