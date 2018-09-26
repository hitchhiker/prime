using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ipfs.Api;
using Nito.AsyncEx;
using Prime.Core;
using Prime.IPFS.Messaging;

namespace Prime.IPFS
{
    public class IpfsInstance
    {
        public readonly IpfsInstanceContext Context;
        public readonly IpfsPlatformBase Platform;
        public readonly IpfsMessenger Messenger;

        public readonly DirectoryInfo WorkspaceDirectory;
        public readonly DirectoryInfo ExecutingDirectory;

        public IpfsInstance(IpfsInstanceContext context)
        {
            Context = context;

            ExecutingDirectory = new FileInfo(GetType().Assembly.Location).Directory;

            WorkspaceDirectory = context.WorkspaceDirectory;
            Platform = context.Platform;

            if (!context.WorkspaceDirectory.Exists)
                context.WorkspaceDirectory.Create();

            if (!IsInstalled())
                context.Platform.Install(this);

            Messenger = new IpfsMessenger(this);

            Daemon.StateChangedAction = m =>
            {
                Messenger.SendIpfsStatus();
            };
            
            Messenger.Start();
        }

        public ILogger L => Context.L;

        private DirectoryInfo _repoDirectory;
        public DirectoryInfo RepoDirectory => _repoDirectory ?? (_repoDirectory = WorkspaceDirectory.EnsureSubDirectory("repo"));

        private DirectoryInfo _nativeExecutableDirectory;
        public DirectoryInfo NativeExecutableDirectory => _nativeExecutableDirectory ?? (_nativeExecutableDirectory = WorkspaceDirectory.EnsureSubDirectory("native"));

        private DirectoryInfo _tempDirectory;
        public DirectoryInfo TempDirectory => _tempDirectory ?? (_tempDirectory = WorkspaceDirectory.EnsureSubDirectory("tmp"));

        private FileInfo _nativeExecutable;
        public FileInfo NativeExecutable => _nativeExecutable ?? (_nativeExecutable = new FileInfo(Path.Combine(NativeExecutableDirectory.FullName, Platform.NativeExecutable)));

        private IpfsDaemonBase _daemon;
        public IpfsDaemonBase Daemon => _daemon ?? (_daemon = Platform.GetDaemon(this));

        private IpfsClient _client;
        public IpfsClient Client => _client ?? (_client = new IpfsClient());

        public void StartDaemon()
        {
            Daemon.Start();
        }

        public void StopDaemon()
        {
            Daemon.Stop();
        }

        private bool IsInstalled()
        {
            return NativeExecutable.Exists;
        }
        
        public bool IsIpfsExternalRunning()
        {
            var client = new IpfsClient();

            try { AsyncContext.Run(() => client.VersionAsync()); }
            catch { return false; }

            return true;
        }

        public bool Do(Action action)
        {
            Daemon.WaitTillRunning(action);
            return true;
        }

        public bool StartAndDo(Action action)
        {
            if (!Daemon.IsRunning())
                Messenger.M.SendAsync(new IpfsStartRequest());

            return Do(action);
        }
    }
}