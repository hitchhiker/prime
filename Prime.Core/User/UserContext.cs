using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using Prime.Core.Encrypt;

//using Prime.Radiant.Components.IPFS.Messenging;

namespace Prime.Core
{
    public class UserContext : IDataContext, IDisposable
    {
        public static string Version = "v0.02";

        public readonly string Token;

        public readonly ObjectId Id;

        public readonly string Username;

        public readonly IList<IUserContextMessenger> Messengers;

        public readonly PrimeContext PrimeContext;

        public UserContext(PrimeContext context, ObjectId id, string username)
        {
            PrimeContext = context;

            Id = id;
            Token = id.ToString();
            Username = username;

            var messengers = TypeCatalogue.I.ImplementInstancesUninitialised<IUserContextMessenger>().ToList();
            Messengers = new List<IUserContextMessenger>();

            foreach (var m in messengers)
                try
                {
                    Messengers.Add(m.GetInstance(this));
                }
                catch (Exception e)
                {
                    Logging.I.DefaultLogger.Fatal(e.Message);
                }
        }

        public static UserContext Testing = new UserContext(PrimeContext.Testing, new ObjectId("50709e6e210a18719ea877a2"), "test");

        public static UserContext Get(ObjectId id)
        {
            //temp
            return id == Testing.Id ? Testing : null;
        }

        ObjectId IDataContext.Id => Id;

        public bool IsPublic => false;

        DirectoryInfo IDataContext.StorageDirectory => StorageDirectoryUsr;
        
        private readonly UserSettings _userdatas = new UserSettings();
        private UserSetting _userSettings;
        public UserSetting UserSettings => _userSettings ?? (_userSettings = _userdatas.GetOrCreate(this));

        private DirectoryInfo _storageDirectory;
        public DirectoryInfo StorageDirectoryUsr => _storageDirectory ?? (_storageDirectory = GetDirInfo());

        private PrimeEncrypt _crypt;
        public PrimeEncrypt Crypt => _crypt ?? (_crypt = new PrimeEncrypt(StorageDirectoryUsr, "usr"));

        private ApiKeys _apiKeys;
        public ApiKeys ApiKeys => _apiKeys ?? (_apiKeys = new ApiKeys(this));

        public ApiKey GetApiKey(INetworkProvider provider) => ApiKeys.GetFirst(provider);

        private DirectoryInfo GetDirInfo()
        {
            var pc = Path.Combine(PrimeContext.FileSystem.UsrDirectory.FullName, Version);
            pc = Path.Combine(pc, Id.ToString());
            var di = new DirectoryInfo(pc);
            if (!di.Exists)
                di.Create();
            return di;
        }

        public void Dispose() { }

        private readonly ConcurrentDictionary<Type, object> _instances  = new ConcurrentDictionary<Type, object>();
        public T GetInstance<T>(Func<T> create) where T : class
        {
            return _instances.GetOrAdd(typeof(T), x => create.Invoke()) as T;
        }
    }
}
