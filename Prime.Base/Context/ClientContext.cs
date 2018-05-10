using System;
using System.IO;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    //Singleton, one per application.

    public class ClientContext
    {
        public readonly IMessenger Messenger;
        public readonly PrimeConfig PrimeConfig;

        public ClientContext() : this("..//..//..//..//instance//prime.config") { } //used for testing / debug

        public ClientContext(string configPath) : this(configPath, DefaultMessenger.I.Default) { }

        public ClientContext(string configPath, IMessenger messenger)
        {
            if (Testing != null)
                throw new Exception(nameof(ClientContext) + " is already initialised in this app domain.");

            if (string.IsNullOrWhiteSpace(configPath))
                throw new ArgumentException($"\'{nameof(configPath)}\' cannot be empty.");

            PrimeConfig = PrimeConfig.Get(Path.GetFullPath(configPath));
            Messenger = messenger;
            Testing = this;
        }

        public static ClientContext Testing { get; private set; }

        private DirectoryInfo _appDataDirectoryInfo;
        public DirectoryInfo AppDataDirectoryInfo => _appDataDirectoryInfo ?? (_appDataDirectoryInfo = GetAppBase());

        private ILogger _logger;
        public ILogger Logger
        {
            get => _logger ?? (_logger = new MessengerLogger(Messenger));
            set => _logger = value;
        }

        private ClientFileSystem _clientFs;
        public ClientFileSystem FileSystem => _clientFs ?? (_clientFs = new ClientFileSystem(this));

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