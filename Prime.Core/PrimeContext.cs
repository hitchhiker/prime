using System;
using System.IO;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    //Singleton, one per application.

    public class PrimeContext
    {
        public readonly IMessenger Messenger;
        public readonly Users Users;
        public readonly PrimeConfig PrimeConfig;

        public PrimeContext() : this("..//..//..//..//instance//prime.config") { } //used for testing / debug

        public PrimeContext(string configPath) : this(configPath, DefaultMessenger.I.Default) { }

        public PrimeContext(string configPath, IMessenger messenger)
        {
            if (string.IsNullOrWhiteSpace(configPath))
                throw new ArgumentException($"\'{nameof(configPath)}\' cannot be empty.");

            PrimeConfig = PrimeConfig.Get(Path.GetFullPath(configPath));
            Messenger = messenger;
            Users = new Users(this);
            Public = new PublicContext(this);
        }

        public static PrimeContext Testing => new PrimeContext();

        public static PublicContext Public;

        private DirectoryInfo _appDataDirectoryInfo;
        public DirectoryInfo AppDataDirectoryInfo => _appDataDirectoryInfo ?? (_appDataDirectoryInfo = GetAppBase());

        private ILogger _logger;
        public ILogger Logger
        {
            get => _logger ?? (_logger = new MessengerLogger(Messenger));
            set => _logger = value;
        }

        private PrimeFileSystem _fileSystem;
        public PrimeFileSystem FileSystem => _fileSystem ?? (_fileSystem = new PrimeFileSystem(this));

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