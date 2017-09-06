using System;
using System.Collections.Generic;
using System.IO;
using SevenZip;
using System.Linq;
using Prime.Utility;

namespace Prime.Radiant.Components
{
    public class PackageBundler
    {
        private readonly PublishManager _publishManager;
        public readonly string ProjectKey;
        public readonly string ProjectPath;
        private readonly PublishManagerContext _context;
        public readonly Logger L;
        public readonly DirectoryInfo SourceDirectory;
        public readonly IReadOnlyList<FileInfo> Files;
        private readonly List<Package> _packed = new List<Package>();

        public string Hash { get; set; }

        public IReadOnlyList<Package> Packed => _packed;
        
        public PackageBundler(PublishManager publishManager, string projectKey, string projectPath)
        {
            SevenZipNativeLibrary.Install();

            _publishManager = publishManager;
            ProjectKey = projectKey;
            ProjectPath = projectPath;
            _context = publishManager.Context;
            L = _context.L;

            SetUpDirectories();

            var dirPath = Path.Combine(_context.SourcePath, ProjectPath);
            if (!Directory.Exists(dirPath))
            {
                L.Info("No project path for " + ProjectKey + " @ " + dirPath);
                return;
            }

            SourceDirectory = new DirectoryInfo(dirPath);
            Files = SourceDirectory.GetFiles("*", SearchOption.AllDirectories).ToList();
        }

        private void Filter(List<FileInfo> items)
        {
            items.RemoveAll(x => !(x.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
                                   x.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
                                   x.Name.EndsWith(".config", StringComparison.OrdinalIgnoreCase)));
        }

        public Package CreatePackage(string packageName, Func<IReadOnlyList<FileInfo>, List<FileInfo>> filter)
        {
            L.Info("Creating package: " + packageName);

            var cores = filter.Invoke(Files);
            return PackGroup(packageName, cores);
        }

        private Package PackGroup(string packageName, List<FileInfo> paths)
        {
            Filter(paths);

            if (paths.Count == 0)
                return new Package();

            L.Info("Packaging " + paths.Count + " objects");

            var compressor = new SevenZipCompressor { ScanOnlyWritable = true };
            var arcname = "arc." + packageName + ".7z";
            var path = Path.Combine(PublishProjectDirectory.FullName, "arc." + packageName + ".7z");

            var old = PublishProjectDirectory.GetFiles("*").Where(x=>x.Name == arcname || x.Name.StartsWith(arcname));
            old.ForEach(x => x.Delete());

            var fi = new FileInfo(path);
            if (fi.Exists)
                fi.Delete();

            L.Info($"Compressing LZMA2 ULTRA: {paths.Count} file(s), {paths.Sum(x=>x.Length)/1024}K ");

            var tocompress = paths.Select(x => x.FullName).ToArray();

            compressor.FastCompression = false;
            compressor.CompressionMode = CompressionMode.Create;
            compressor.CompressionLevel = CompressionLevel.Ultra;
            compressor.CompressionMethod = CompressionMethod.Lzma2;
            compressor.VolumeSize = 1024 * 1024 * 1;
            compressor.CompressFiles(path, tocompress);

            L.Info("Complete");

            fi.Refresh();
            var pack = new Package(packageName, paths, fi);
            _packed.Add(pack);
            return pack;
        }

        public DirectoryInfo PublishProjectDirectory { get; private set; }

        private void SetUpDirectories()
        {

            var di = CommonFs.I.GetCreateUsrSubDirectory("publish");
            if (!di.Exists)
                di.Create();

            PublishProjectDirectory = new DirectoryInfo(Path.Combine(di.FullName, ProjectKey));
            if (!PublishProjectDirectory.Exists)
                PublishProjectDirectory.Create();
        }
    }
}