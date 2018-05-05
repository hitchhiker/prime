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

        public PrimeContext() : this(DefaultMessenger.I.Default)
        {

        }

        public PrimeContext(IMessenger messenger)
        {
            Messenger = messenger;
            Users = new Users(this);
            Public = new PublicContext(this);
        }

        public static PrimeContext Testing => new PrimeContext();

        public static PublicContext Public;

        private DirectoryInfo _workContainerDirectoryInfo;
        public DirectoryInfo WorkContainerDirectoryInfo
        {
            get => _workContainerDirectoryInfo ?? (_workContainerDirectoryInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)));
            set =>  _workContainerDirectoryInfo = value;
        }

        private ILogger _logger;
        public ILogger Logger
        {
            get => _logger ?? (_logger = new MessengerLogger(Messenger));
            set => _logger = value;
        }

        private PrimeFileSystem _fileSystem;
        public PrimeFileSystem FileSystem => _fileSystem ?? (_fileSystem = new PrimeFileSystem(this));
    }
}