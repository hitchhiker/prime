using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prime.Base;
using Prime.Core;

namespace Prime.ExtensionPackager
{
    public class PackageBundler
    {
        public static readonly string ArchiveName = "arc.bz2";

        private readonly Package _package;
        public readonly ProgramContext Context;
        private int _inc = 1;

        public PackageBundler(Package package, ProgramContext context)
        {
            _package = package;
            Context = context;
        }

        public void Bundle()
        {
            if (_package.StagedFiles?.Count != 0)
                Process();
            else
                Context.Logger.Warn("No staged files to process.");
        }

        private void Process()
        {
            Filter(_package.StagedFiles);
            var vpOffset = _package.StagedRoot.FullName.Length;

            var remaining = _package.StagedFiles.ToList();

            var distDir = new DirectoryInfo(Path.Combine(Context.DistributionDirectory.FullName, _package.GetDirectory()) + Path.DirectorySeparatorChar);
            if (distDir.Exists)
                distDir.Delete(true);

            distDir.Create();

            //move any archives, avoiding re-compression, increase probability of existing hash hit, retain identical folder structure.

            MoveOnly(distDir, vpOffset, remaining);

            //organise files into groups to reduce probability of re-transmission of unchanged files (hash hit) in the future.

            CompressToArchive(distDir, remaining.Where(x => x.Length > 1024 * 150), remaining, true);

            CompressToArchive(distDir, remaining.Where(x => x.Name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase)), remaining);
            CompressToArchive(distDir, remaining.Where(x => x.Name.StartsWith("System.", StringComparison.OrdinalIgnoreCase)), remaining);

            CompressToArchive(distDir, remaining.Where(x => x.Length< 1024 * 30), remaining);

            CompressToArchive(distDir, FilterMod3(remaining, 1), remaining);
            CompressToArchive(distDir, FilterMod3(remaining, 2), remaining);
            CompressToArchive(distDir, remaining, remaining);

            var dMetaPath = Path.Combine(distDir.FullName, PackageMetaInspector.ExtFileName);
            _package.StagedMeta.CopyTo(dMetaPath, true);

            Context.Logger.Info("Compression complete.");
        }

        private void Filter(List<FileInfo> items)
        {
            items.RemoveAll(x => x.Name.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase));
        }

        private void MoveOnly(DirectoryInfo distDir, int vpOffset, List<FileInfo> remaining)
        {
            var items = remaining.Where(x => string.Equals(x.Extension, ".zip", StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(x.Extension, ".gz", StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(x.Extension, ".rar", StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(x.Extension, ".7z", StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(x.Extension, ".lz", StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(x.Extension, ".lzma", StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(x.Extension, ".bz2", StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(x.Extension, ".z", StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(x.Extension, ".arj", StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(x.Extension, ".tar", StringComparison.OrdinalIgnoreCase)).ToList();


            foreach (var fi in items)
            {
                var vp = fi.FullName.Substring(vpOffset);

                if (vp.StartsWith(Path.DirectorySeparatorChar))
                    vp = vp.Substring(1);

                var dst = new FileInfo(Path.Combine(distDir.FullName, vp));

                if (!dst.Directory.Exists)
                    dst.Directory.Create();

                fi.CopyTo(dst.FullName, true);
            }

            Context.Logger.Info($"Moved {items.Count} existing archives(s).");

            remaining.RemoveAll(items.Contains);
        }

        private void CompressToArchive(DirectoryInfo distDir, IEnumerable<FileInfo> src, List<FileInfo> remaining, bool asSingle = false)
        {
            var items = src.ToList();

            if (items.Count == 0)
                return;

            if (asSingle)
                foreach (var i in items)
                    _package.StagedRoot.CreateArchive(new List<FileInfo> {i}, Path.Combine(distDir.FullName, $"{ArchiveName}.{_inc++}"));
            else
                _package.StagedRoot.CreateArchive(items.OrderBy(x => x.Name), Path.Combine(distDir.FullName, $"{ArchiveName}.{_inc++}"));
            
            Context.Logger.Info($"Compressed {items.Count} files(s).");
            remaining.RemoveAll(items.Contains);
        }

        public IEnumerable<FileInfo> FilterMod3(IEnumerable<FileInfo> files, int position)
        {
            return files.Where(x => Math.Abs(x.Name.GetHashCode()) % 3 == position);
        }
    }
}