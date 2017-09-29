using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prime.Core;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class ApiKeys : IEnumerable<ApiKey>
    {
        private readonly UserContext _context;
        private readonly UniqueList<ApiKey> _keys;

        internal ApiKeys(UserContext context)
        {
            _context = context;
            _keys = _context.GetCollection<ApiKey>().FindAll().ToUniqueList();
            CheckFileSystem();
        }

        public IReadOnlyList<ApiKey> Keys => _keys;

        public LiteCollection<ApiKey> Collection()
        {
            return _context.GetCollection<ApiKey>();
        }

        public void Clear()
        {
            var col = Collection();
            col.Delete(x => true);
        }

        public ApiKey GetFirst(INetworkProvider provider)
        {
            return GetFirst(provider.Network);
        }

        public ApiKey GetFirst(Network network)
        {
            return _keys.FirstOrDefault(x => Equals(x.Network, network));
        }

        private void CheckFileSystem()
        {
            if (_keys.Any() || !ApiConfigPath.Exists)
                return;

            var files = ApiConfigPath.GetFiles("*.api.txt", SearchOption.AllDirectories);
            if (!files.Any())
                return;

            CollectFilesystem(files);
        }

        private void CollectFilesystem(FileInfo[] files)
        {
            foreach (var f in files)
            {
                var networkName = f.Name.Replace(".api.txt", "").Trim();
                var network = Networks.I.FirstOrDefault(x => x.NameLowered == networkName);
                if (network == null)
                {
                    Logging.I.DefaultLogger.Error("Unable to network: " + networkName + " while collecting api keys from configuration folder.");
                    continue;
                }
                CollectFilesystem(network, f);
            }

            if (_keys.Any())
                Save();
        }

        public void Save()
        {
            _keys.SaveAll(_context);
        }

        private void CollectFilesystem(Network network, FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
                return;

            var apik = ApiKey.FromFile(network, fileInfo);
            if (apik == null)
                return;

            _keys.Add(apik);
        }

        public void Remove(ApiKey key)
        {
            _keys.Remove(key);
        }

        public void Add(ApiKey key)
        {
            _keys.Add(key, true);
        }

        public static DirectoryInfo ApiConfigPath = new DirectoryInfo(Path.Combine(CommonFs.I.UserConfigDirectory.FullName, "keys"));

        public IEnumerator<ApiKey> GetEnumerator()
        {
            return _keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _keys).GetEnumerator();
        }
    }
}
