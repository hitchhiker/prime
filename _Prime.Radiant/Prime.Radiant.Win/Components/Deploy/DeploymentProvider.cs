using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.IO.Compression;
using Prime.Utility;
using SevenZip;

namespace Prime.Radiant
{
    public class DeploymentProvider
    {
        public DeploymentProvider(string projectKey, string hash, ILogger logger)
        {
            SevenZipNativeLibrary.Install();

            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException(nameof(hash));

            if (string.IsNullOrWhiteSpace(projectKey))
                throw new ArgumentException(nameof(projectKey));

            _projectKey = projectKey;
            Hash = hash;
            L = logger;
            
            var di = CommonFs.I.GetCreateUsrSubDirectory("deploy");

            ProjectDirectory = CommonFs.I.GetCreateSubDirectory(di, _projectKey);
            AssetDirectory = CommonFs.I.GetCreateSubDirectory(ProjectDirectory, "_" + hash);
            BinDirectory = CommonFs.I.GetCreateSubDirectory(AssetDirectory, "bin");

            RetrievedMarker = new FileInfo(Path.Combine(AssetDirectory.FullName, ".retrieved"));
            IsRetrieved = AssetDirectory.Exists && RetrievedMarker.Exists;

            Trim();
        }

        public const int RetentionAmount = 2;

        private readonly string _projectKey;

        public readonly string Hash;

        private readonly ILogger L;

        public readonly DirectoryInfo AssetDirectory;

        public readonly DirectoryInfo BinDirectory;

        public DirectoryInfo ProjectDirectory { get; private set; }

        public readonly FileInfo RetrievedMarker;

        public bool IsRetrieved { get; private set; }

        public void SetComplete()
        {
            File.WriteAllText(RetrievedMarker.FullName, DateTime.UtcNow.Ticks.ToString());
            RetrievedMarker.Refresh();
            IsRetrieved = AssetDirectory.Exists && RetrievedMarker.Exists;
            L.Info("Decentralised deployment complete");
        }

        public string GetFromStream(Stream ipfsStream, long size, string name)
        {
            L.Info("Stream retrieved " + size + " bytes");

            var path = Path.Combine(AssetDirectory.FullName, name);

            using (var file = File.Create(path))
            {
                CopyStream(ipfsStream, file, size);
            }

            return path;
        }

        public void Decompress() {

            if (!BinDirectory.Exists)
                BinDirectory.Create();

            var items = AssetDirectory.GetFiles().ToList();

            if (items.Count == 1)
            {
                L.Info("Decompressing stream");

                using (var arc = new SevenZipExtractor(items[0].FullName))
                    arc.ExtractArchive(BinDirectory.FullName); // extract all
                File.Delete(items[0].FullName);
            }

            var names = items.Select(x => x.Name.Replace(x.Extension, "")).Distinct().ToList();

            foreach (var i in names)
            {
                L.Info("Decompressing " + i);

                using (var arc = new SevenZipExtractor(Path.Combine(AssetDirectory.FullName, i + ".001")))
                    arc.ExtractArchive(BinDirectory.FullName); // extract all
            }

            items.ForEach(x => x.Delete());
        }

        public void CopyStream(Stream input, Stream output, long size)
        {
            //var t = DateTime.MinValue;
            //var s = TimeSpan.FromSeconds(1);

            var buffer = new byte[32 * 1024];
            int len;
            //double total = 0;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
                //total += len;

                continue;
                /*
                if (!t.IsStale(s))
                    continue;

                _logger(total + " / " + size + " (" + Math.Round((100D/size)*total, 2) + "%)");
                t = DateTime.UtcNow;*/
            }
        }

        public void Trim()
        {
            var dirPaths = Directory.EnumerateDirectories(ProjectDirectory.FullName, "_*", new SearchOption()).Where(x => x!=AssetDirectory.FullName).ToList();

            if (dirPaths.Count<= RetentionAmount)
                return;

            var dirs = dirPaths.Select(x => new DirectoryInfo(x)).ToList();
            var toRemove = dirs.Count - RetentionAmount;

            L.Info("Cleaning up " + toRemove + " older deployment(s).");

            foreach (var d in dirs.OrderBy(x => x.CreationTimeUtc).Take(toRemove))
                d.Delete(true);
        }
    }
}
