using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Core
{
    public class Prime
    {
        private Prime()
        {
            StartupMessengers = TypeCatalogue.I.ImplementInstances<IStartupMessenger>().ToList();
        }

        public static Prime I => Lazy.Value;
        private static readonly Lazy<Prime> Lazy = new Lazy<Prime>(()=>new Prime());

        public readonly List<IStartupMessenger> StartupMessengers;

        private DirectoryInfo _storageDirectory;

        public DirectoryInfo StorageDirectory => _storageDirectory ?? (_storageDirectory = GetSDir());

        private DirectoryInfo GetSDir()
        {
            var di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"Prime"));
            if (!di.Exists)
                di.Create();
            return di;
        }

        public static IDictionary<Asset, uint> DecimalPlaces = GetDecimals();

        private static Dictionary<Asset, uint> GetDecimals()
        {
            var d = new Dictionary<Asset, uint>
            {
                {"USD".ToAssetRaw(), 3},
                {"EUR".ToAssetRaw(), 3},
                {"USDT".ToAssetRaw(), 5}
            };
            return d;
        }

        public IWindowManager GetWindowManager(UserContext context)
        {
            return TypeCatalogue.I.ImplementInstancesWith<IWindowManager>(context).FirstOrDefault();
        }
    }
}
