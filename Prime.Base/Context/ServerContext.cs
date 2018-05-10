using System;
using System.IO;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    //Singleton, one per application.

    public class ServerContext
    {
        public readonly IMessenger Messenger;
        public readonly Users Users;
        public readonly PrimeConfig PrimeConfig;

        public ServerContext() : this("..//..//..//..//instance//prime.config") { } //used for testing / debug

        public ServerContext(string configPath) : this(configPath, DefaultMessenger.I.Default) { }

        public ServerContext(string configPath, IMessenger messenger)
        {
            if (Testing != null)
                throw new Exception(nameof(ServerContext) + " is already initialised in this app domain.");

            if (string.IsNullOrWhiteSpace(configPath))
                throw new ArgumentException($"\'{nameof(configPath)}\' cannot be empty.");

            PrimeConfig = PrimeConfig.Get(Path.GetFullPath(configPath));
            Messenger = messenger;
            Users = new Users(this);
            Public = new PublicContext(this);
            Testing = this;
        }

        public static ServerContext Testing { get; private set; }

        public static PublicContext Public;

        private DirectoryInfo _appDataDirectoryInfo;
        public DirectoryInfo AppDataDirectoryInfo => _appDataDirectoryInfo ?? (_appDataDirectoryInfo = GetAppBase());

        private ILogger _logger;
        public ILogger Logger
        {
            get => _logger ?? (_logger = new MessengerLogger(Messenger));
            set => _logger = value;
        }

        private ServerFileSystem _serverFileSystem;
        public ServerFileSystem FileSystem => _serverFileSystem ?? (_serverFileSystem = new ServerFileSystem(this));

        private DirectoryInfo GetAppBase()
        {
            if (!string.IsNullOrWhiteSpace(PrimeConfig.BasePath))
                return new DirectoryInfo(Path.GetFullPath(PrimeConfig.BasePath));

            if (PrimeConfig.ConfigLoadedFrom!=null)
                return PrimeConfig.ConfigLoadedFrom.Directory;

            return new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        }
    }
}