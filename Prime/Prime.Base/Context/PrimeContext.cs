using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    /// <summary>
    /// Singleton, one per application.
    /// </summary>
    public class PrimeContext : IPrimeContext
    {
        public readonly IMessenger M;
        public readonly Users Users;
        public readonly PrimeConfig Config;
        public readonly DirectoryInfo ConfigDirectoryInfo;
        public readonly Platform PlatformCurrent;
        public readonly AssemblyCatalogue Assemblies;
        public readonly TypeCatalogue Types;
        public IReadOnlyList<IExtension> Extensions => _extensions;
        private readonly List<IExtension> _extensions = new List<IExtension>();
        public readonly string Name;

        public static PrimeContext ForDevelopment(string configName) => new PrimeContext("[src]/instance/" + configName) { L = new ConsoleLogger() };

        public static PrimeContext ForDevelopmentClient() => ForDevelopment("prime-client.config");

        public static PrimeContext ForDevelopmentServer() => ForDevelopment("prime-server.config");

        public PrimeContext(string configPath) : this(configPath, DefaultMessenger.I.Instance) { }

        public PrimeContext(string configPath, IMessenger m)
        {
            configPath = configPath.ResolveSpecial();

            PlatformCurrent = OsInformation.GetPlatform();

            _testing = this; //todo: hack for now.

            if (string.IsNullOrWhiteSpace(configPath))
                throw new ArgumentException($"\'{nameof(configPath)}\' cannot be empty.");

            Assemblies = new AssemblyCatalogue();
            Types = new TypeCatalogue(Assemblies);
            var fullpath = Path.GetFullPath(configPath);
            var configFi = new FileInfo(fullpath);

            ConfigDirectoryInfo = configFi.Directory;
            Config = PrimeConfig.Get(fullpath);
            M = m;
            Users = new Users(this);
            Public = new PublicContext(this);
            Name = configFi.Name.Replace(configFi.Extension, "");
        }

        public void AddInitialisedExtension(IExtension extension)
        {
            _extensions.RemoveAll(x => x.Id == extension.Id);
            _extensions.Add(extension);
        }

        private static PrimeContext _testing = ForDevelopmentServer();
        public static PrimeContext Testing => _testing;

        public static PublicContext Public;

        private DirectoryInfo _appDataDirectoryInfo;
        public DirectoryInfo AppDataDirectoryInfo => _appDataDirectoryInfo ?? (_appDataDirectoryInfo = GetAppBase());

        private ILogger _logger;
        public ILogger L
        {
            get => _logger ?? (_logger = new MessengerLogger(M));
            set => _logger = value;
        }

        private PrimeFileSystem _primeFileSystem;
        public PrimeFileSystem FileSystem => _primeFileSystem ?? (_primeFileSystem = new PrimeFileSystem(this));

        private DirectoryInfo GetAppBase()
        {
            if (!string.IsNullOrWhiteSpace(Config.BasePath))
                return new DirectoryInfo(Path.GetFullPath(Config.BasePath));

            if (Config.ConfigLoadedFrom != null)
                return Config.ConfigLoadedFrom.Directory;

            return new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        }
    }
}