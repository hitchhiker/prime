using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class PublicContext : IDataContext
    {
        public static string Version = "v0.01"; 

        public static PublicContext I => Lazy.Value;
        private static readonly Lazy<PublicContext> Lazy = new Lazy<PublicContext>(()=>new PublicContext());

        private PublicContext()
        {
            Id = "prime:public:context".GetObjectIdHashCode();
        }

        public readonly ObjectId Id;

        ObjectId IDataContext.Id => Id;

        DirectoryInfo IDataContext.StorageDirectory => StorageDirectoryPub;

        public bool IsPublic => true;

        private LiteQueryable<NetworkData> _qNetworkData;
        public LiteQueryable<NetworkData> QNetworkData => _qNetworkData ?? (_qNetworkData = this.As<NetworkData>());

        private AssetInfos _assetInfos;
        public AssetInfos AssetInfos => _assetInfos ?? (_assetInfos = AssetInfos.Get());

        private DirectoryInfo _storageDirectory;
        public DirectoryInfo StorageDirectoryPub => _storageDirectory ?? (_storageDirectory = GetDirInfo());

        private DirectoryInfo GetDirInfo()
        {
            var pc = Path.Combine(PrimeCommon.I.Environment.StorageDirectory.FullName, "pub");
            pc = Path.Combine(pc, Version);
            var di = new DirectoryInfo(pc);
            if (!di.Exists)
                di.Create();
            return di;
        }

        private static readonly PublicDatas PublicDatas = new PublicDatas();

        private PublicData _pubData;
        public PublicData PubData => _pubData ?? (_pubData = PublicDatas.GetOrCreate(this));
        
        private ExchangeDatas _exchangeData;
        public ExchangeDatas ExchangeDatas => _exchangeData ?? (_exchangeData = new ExchangeDatas());

        public ExchangeData Data(IExchangeProvider provider) => ExchangeDatas.GetOrCreate(this, provider);

        private readonly object _singletonLock = new object();
        private readonly List<object> _singletons = new List<object>();

        public T GetOrAddSingleton<T>(Func<T> create)
        {
            lock (_singletonLock)
            {
                var vm = _singletons.OfType<T>().FirstOrDefault();
                if (vm != null)
                    return vm;

                vm = create.Invoke();
                _singletons.Add(vm);
                return vm;
            }
        }
    }
}
