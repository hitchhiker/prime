using Prime.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using Ipfs.Api;
using Nito.AsyncEx;
using Timer = System.Timers.Timer;

namespace Prime.Radiant
{
    public class IpFsDaemon
    {
        private readonly Dispatcher _dispatcher;
        public readonly ILogger L;

        public event EventHandler OnStateChanged;

        private Process _process;
        private bool _requiresInit;
        private bool _lockWait;
        private Timer _externalPollTimer;
        private IpFsDaemonState _state;
        private ExecuteDos.DosContext _dosContext;

        public bool RedirectRepository { get; set; }

        public bool AutoRestart { get; set; } = true;

        public readonly IpfsClient Client;

        public IpFsDaemon(ILogger logger = null, Dispatcher dispatcher = null)
        {
            _dispatcher = dispatcher;
            L = logger ?? Logging.I.DefaultLogger;
            State = IpFsDaemonState.Stopped;
            Client = new IpfsClient();
        }

        public IpFsDaemon(DeploymentManager manager) : this(manager.L, manager.Dispatcher)
        {
        }

        public IpFsDaemonState State
        {
            get => _state;
            private set
            {
                _state = value;
                OnStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void WaitTillRunning(Action<IpfsClient> action)
        {
            Start();

            enjoyGotoPurists:

            if (State == IpFsDaemonState.Running || State == IpFsDaemonState.System)
            {
                action.Invoke(Client);
                return;
            }

            Thread.Sleep(1);

            goto enjoyGotoPurists;
        }

        public T WaitTillRunning<T>(Func<IpfsClient, T> function)
        {
            Start();

            enjoyGotoPurists:

            if (State == IpFsDaemonState.Running || State == IpFsDaemonState.System)
                return function.Invoke(Client);

            Thread.Sleep(1);

            goto enjoyGotoPurists;
        }

        private volatile bool _isStarted;

        public void Start()
        {
            if (_isStarted && State!= IpFsDaemonState.Stopped)
                return;

            _isStarted = true;
            StartInternal(true);
        }

        private void StartInternal(bool allowInitialisation)
        {
            if (IsIpfsExternalRunning())
            {
                InitForExternal();
                return;
            }

            StartLocal(allowInitialisation);
        }

        public void Stop()
        {
            if (State != IpFsDaemonState.Running)
                return;

            _externalPollTimer?.Close();

            if (_dosContext != null)
                _dosContext.Cancelled = true;
            else
                _process?.Kill();

            State = IpFsDaemonState.Stopping;
        }

        private bool IsIpfsExternalRunning()
        {
            var client = new IpfsClient();

            try { AsyncContext.Run(() => client.VersionAsync()); }
            catch { return false; }

            return true;
        }

        private void InitForExternal()
        {
            L.Info("IPFS is already running on this machine, we're using that instance.");
            State = IpFsDaemonState.System;
            _externalPollTimer = new Timer
            {
                Interval = 1000 * 2,
                AutoReset = false,
                Enabled = true
            };

            _externalPollTimer.Elapsed += ExternalPollTimerElapsed;
        }

        private void ExternalPollTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsIpfsExternalRunning())
            {
                State = IpFsDaemonState.Stopped;
                if (AutoRestart)
                    Start();
                return;
            }

            _externalPollTimer.Enabled = true;
        }

        private void StartLocal(bool allowInitialisation = true)
        {
            _requiresInit = false;
            _lockWait = false;

            State = IpFsDaemonState.Starting;

            var processContext = new IpfsProcessContext("daemon",
                message =>
                {
                    if (message.Contains("daemon is ready", StringComparison.OrdinalIgnoreCase))
                        State = IpFsDaemonState.Running;
                    if (message.Contains("interrupt signal", StringComparison.OrdinalIgnoreCase))
                    {
                        State = IpFsDaemonState.Stopping;
                        return DosCancellation.StopLogging;
                    }
                    return DosCancellation.None;
                },
                error =>
                {
                    if (error.Contains("'ipfs init'", StringComparison.OrdinalIgnoreCase) ||
                        error.Contains("no IPFS repo found", StringComparison.OrdinalIgnoreCase))
                    {
                        _requiresInit = true;
                        State = IpFsDaemonState.Stopped;
                        return DosCancellation.Terminate;
                    }

                    if (!error.Contains("already locked"))
                        return DosCancellation.None;

                    _lockWait = true;
                    State = IpFsDaemonState.Stopped;
                    return DosCancellation.Terminate;
                },
                process => { _process = process; })
            {
                OnProcessEnded = () =>
                {
                   // if (AutoRestart && !_requiresInit && !_lockWait)
                    //    Start();
                }
            };

            var task = IssueIpfsNativeCommand(processContext);

            if (task == null)
                State = IpFsDaemonState.Stopped;

            task.ContinueWith(task1 => FinalStep(task1, allowInitialisation));

            task.Start();
        }

        private Task FinalStep(Task<ExecuteDos.ProcessResult> pr, bool allowInitialisation)
        {
            State = IpFsDaemonState.Stopped;

            if (_requiresInit && allowInitialisation)
            {
                L.Info("IPFS repo requires init");
                return DoRepositoryInitialisation().ContinueWith(ok => StartInternal(false));
            }

            if (_lockWait)
            {
                L.Info("IPFS repo is locked (we're waiting for the unlock)");
                Thread.Sleep(5000);
                StartInternal(false);
            }

            return null;
        }

        private async Task<bool> DoRepositoryInitialisation()
        {
            L.Info("Initialising IPFS Repository on this machine.");

            var initok = false;

            var initResult = await InitialiseRepository(() => initok = true);

            return initResult && initok;
        }

        private async Task<bool> InitialiseRepository(Action onSuccess)
        {
            var task = IssueIpfsNativeCommand(new IpfsProcessContext("init", message =>
            {
                if (!message.Contains("peer identity"))
                    return DosCancellation.None;

                onSuccess.Invoke();
                return DosCancellation.StopLogging;
            }, error => DosCancellation.StopLogging));

            if (task == null)
                return false;

            task.Start();

            var pr = await task;

            return pr?.Success == true;
        }

        private Task<ExecuteDos.ProcessResult> IssueIpfsNativeCommand(IpfsProcessContext ipfsProcessContext)
        {
            var ipfsexe = new FileInfo(IpfsExePath);

            if (!ipfsexe.Exists)
            {
                L.Info("Could not find ipfs.exe @ " + ipfsexe.FullName);
                return null;
            }

            var dosContext = _dosContext = new ExecuteDos.DosContext(ipfsexe.FullName, ipfsProcessContext.Command)
            {
                OnProcessCreated = ipfsProcessContext.OnProcessCreated,
                OnProcessEnded = ipfsProcessContext.OnProcessEnded,
                TimeOut = TimeSpan.MaxValue,
                RedirectStandardInput = true
            };

            var stoplogging = false;

            dosContext.Logger = message =>
            {
                if (message != null)
                {
                    var c = ipfsProcessContext.CheckLog(message);
                    if (c == DosCancellation.Terminate)
                        dosContext.Cancelled = true;
                    if (c == DosCancellation.StopLogging || c == DosCancellation.Terminate)
                        stoplogging = true;
                }

                if (!stoplogging && !string.IsNullOrEmpty(message))
                    L.Info(message);
            };

            dosContext.ErrorLogger = error =>
            {
                if (error != null)
                {
                    var c = ipfsProcessContext.CheckError(error);
                    if (c == DosCancellation.Terminate)
                        dosContext.Cancelled = true;
                    if (c == DosCancellation.StopLogging || c == DosCancellation.Terminate)
                        stoplogging = true;
                }

                if (!stoplogging && !string.IsNullOrEmpty(error))
                    L.Error(error);
            };

            if (RedirectRepository)
                dosContext.Environment.Add("IPFS_PATH", RepoDirectory.FullName);

            if (_dispatcher != null)
                _dispatcher.Invoke(() => BindExit(dosContext));
            else
                BindExit(dosContext);
            
            return new ExecuteDos().CmdAsync(dosContext);
        }

        private DirectoryInfo _ipfsDirectory;
        public DirectoryInfo IpfsDirectory => _ipfsDirectory ?? (_ipfsDirectory = CommonFs.I.GetCreateUsrSubDirectory("ipfs"));

        private DirectoryInfo _repoDirectory;
        public DirectoryInfo RepoDirectory => _repoDirectory ?? (_repoDirectory = CommonFs.I.GetCreateSubDirectory(IpfsDirectory, "repo"));

        private DirectoryInfo _staticExeDirectory;
        public DirectoryInfo StaticExeDirectory => _staticExeDirectory ?? (_staticExeDirectory = CommonFs.I.GetCreateSubDirectory(IpfsDirectory, "native"));

        private static void BindExit(ExecuteDos.DosContext dosContext)
        {
            if (Application.Current != null)
                Application.Current.Exit += delegate
                {
                    if (dosContext != null)
                        dosContext.Cancelled = true;
                };
        }

        private string _ipfsExePath;
        public string IpfsExePath => _ipfsExePath ?? (_ipfsExePath = GetIpfsExe());

        /// <summary>
        /// We copy to the USR location so that windows et al. won't bother us with constant firewall requests.
        /// </summary>
        /// <returns></returns>
        
        [Obsolete("Doesn't take versions into account")]
        private string GetIpfsExe()
        {
            var cpuExe = Environment.Is64BitOperatingSystem ? "ipfs-x64.exe" : "ipfs-x86.exe";
            var native = StaticExeDirectory;
            var staticExePath = Path.Combine(native.FullName, cpuExe);

            if (File.Exists(staticExePath))
                return staticExePath;

            var binDirectory = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory);
            var binLocation = Path.Combine(binDirectory.FullName, cpuExe);

            if (!File.Exists(binLocation))
                throw new Exception(binLocation + " is not found.");

            File.Copy(Path.Combine(binDirectory.FullName, cpuExe), staticExePath, true);
            return staticExePath;
        }
    }
}