using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using Prime.Core.Annotations;
using Prime.Core.Wallet;
using Prime.Utility;
using Prime.Utility.Encrypt;
using LiteDB;
using Prime.Radiant.Components.IPFS.Messenging;

namespace Prime.Core
{
    public class UserContext : NotifierBase, IDataContext, IDisposable
    {
        public static string Version = "v0.02";

        public UserContext(ObjectId id, string username)
        {
            Id = id;
            Username = username;
        }

        public readonly ObjectId Id;

        public readonly string Username;

        private readonly UserSettings _userdatas = new UserSettings();

        public static UserContext Current = new UserContext(new ObjectId("50709e6e210a18719ea877a2"), "test");

        ObjectId IDataContext.Id => Id;

        public bool IsPublic => false;

        private Asset _baseAsset;
        public Asset BaseAsset
        {
            get => _baseAsset ?? (_baseAsset = "USD".ToAssetRaw());
            set => Set(ref _baseAsset, value);
        }

        DirectoryInfo IDataContext.StorageDirectory => StorageDirectoryUsr;

        private IWindowManager _windowManager;
        public IWindowManager WindowManager => _windowManager ?? (_windowManager = Prime.I.GetWindowManager(this));

        private UserSetting _userSettings;
        public UserSetting UserSettings => _userSettings ?? (_userSettings = _userdatas.GetOrCreate(this));

        private WalletDatas _walletDatas;
        public WalletDatas WalletDatas => _walletDatas ?? (_walletDatas = new WalletDatas());

        private ProviderDatas _providerDatas;
        public ProviderDatas ProviderDatas => _providerDatas ?? (_providerDatas = new ProviderDatas(this));
        
        public WalletData Data(IWalletService provider) => WalletDatas.GetOrCreate(this, provider);

        public ProviderData Data(INetworkProvider provider) => ProviderDatas.Get(provider);

        private DirectoryInfo _storageDirectory;
        public DirectoryInfo StorageDirectoryUsr => _storageDirectory ?? (_storageDirectory = GetDirInfo());

        private PrimeEncrypt _crypt;
        public PrimeEncrypt Crypt => _crypt ?? (_crypt = new PrimeEncrypt(StorageDirectoryUsr, "usr"));

        private IpfsMessenger _ipfsMessenger;
        public IpfsMessenger IpfsMessenger => _ipfsMessenger ?? (_ipfsMessenger = new IpfsMessenger(Radiant, Id.ToString(), Crypt));

        private Radiant.Radiant _radiant;
        public Radiant.Radiant Radiant => _radiant ?? (_radiant = new Radiant.Radiant(Logging.I.DefaultLogger));

        private PortfolioProvider _portfolioProvider;
        public PortfolioProvider PortfolioProvider => _portfolioProvider ?? (_portfolioProvider = new PortfolioProvider(this));

        private WalletProvider _walletProvider;
        public WalletProvider WalletProvider => _walletProvider ?? (_walletProvider = new WalletProvider(this));

        public ApiKey GetApiKey(INetworkProvider provider)
        {
            return Data(provider).ApiKeys.GetFirst(provider);
        }

        private DirectoryInfo GetDirInfo()
        {
            var pc = Path.Combine(Prime.I.StorageDirectory.FullName, "usr");
            pc = Path.Combine(pc, Version);
            pc = Path.Combine(pc, Id.ToString());
            var di = new DirectoryInfo(pc);
            if (!di.Exists)
                di.Create();
            return di;
        }

        public CommandBase LastCommand { get; set; }

        public void Dispose() { }

        private readonly object _singletonLock = new object();
        private readonly List<object> _singletons  = new List<object>();

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
