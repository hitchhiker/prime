using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;
using Prime.Core;
using Prime.Core.Windows;

namespace Prime.IPFS
{
    public class IpfsWindowsDaemon : IpfsDaemonBase
    {
        private readonly IpfsInstance _instance;

        public event EventHandler OnStateChanged;

        private Process _process;
        private bool _requiresInit;
        private bool _lockWait;
        private Timer _externalPollTimer;
        private IpFsDaemonState _currentState;
        private ExecuteDos.DosContext _dosContext;

        public bool RedirectRepository { get; set; } = true;

        public bool AutoRestart { get; set; } = true;

        public IpfsWindowsDaemon(IpfsInstance instance) : base(instance)
        {
            _instance = instance;
            CurrentState = IpFsDaemonState.Stopped;
        }

        public ILogger L => _instance.L;

        private IpFsDaemonState CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                OnStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private volatile bool _isStarted;

        public override IpFsDaemonState State()
        {
            return CurrentState;
        }

        public override void Start()
        {
            if (_isStarted && CurrentState!= IpFsDaemonState.Stopped)
                return;

            _isStarted = true;
            StartInternal(true);
        }

        private void StartInternal(bool allowInitialisation)
        {
            if (Instance.IsIpfsExternalRunning())
            {
                InitForExternal();
                return;
            }

            StartLocal(allowInitialisation);
        }

        public override void Stop()
        {
            if (CurrentState != IpFsDaemonState.Running)
                return;

            _externalPollTimer?.Close();

            if (_dosContext != null)
                _dosContext.Cancelled = true;
            else
                _process?.Kill();

            CurrentState = IpFsDaemonState.Stopping;
        }


        private void InitForExternal()
        {
            L.Info("IPFS is already running on this machine, we're using that instance.");
            CurrentState = IpFsDaemonState.System;
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
            if (!Instance.IsIpfsExternalRunning())
            {
                CurrentState = IpFsDaemonState.Stopped;
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

            CurrentState = IpFsDaemonState.Starting;

            var processContext = new DosProcessContext("daemon --init",
                message =>
                {
                    if (message.Contains("Daemon is ready", StringComparison.OrdinalIgnoreCase))
                        CurrentState = IpFsDaemonState.Running;
                    if (message.Contains("interrupt signal", StringComparison.OrdinalIgnoreCase))
                    {
                        CurrentState = IpFsDaemonState.Stopping;
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
                        CurrentState = IpFsDaemonState.Stopped;
                        return DosCancellation.Terminate;
                    }

                    if (!error.Contains("already locked"))
                        return DosCancellation.None;

                    _lockWait = true;
                    CurrentState = IpFsDaemonState.Stopped;
                    return DosCancellation.Terminate;
                },
                process => { _process = process; })
            {
                OnProcessEnded = () =>
                {
                    CurrentState = IpFsDaemonState.Stopped;

                    // if (AutoRestart && !_requiresInit && !_lockWait)
                    //    Start();
                }
            };

            var task = IssueIpfsNativeCommand(processContext);

            if (task == null)
                CurrentState = IpFsDaemonState.Stopped;

            task.ContinueWith(task1 => FinalStep(task1, allowInitialisation));

            task.Start();
        }

        private Task FinalStep(Task<ExecuteDos.ProcessResult> pr, bool allowInitialisation)
        {
            CurrentState = IpFsDaemonState.Stopped;

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
            var task = IssueIpfsNativeCommand(new DosProcessContext("init", message =>
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

        private Task<ExecuteDos.ProcessResult> IssueIpfsNativeCommand(DosProcessContext ipfsProcessContext)
        {
            var ipfsexe = Instance.NativeExecutable;

            if (!ipfsexe.Exists)
            {
                L.Info("Could not find ipfs executable @ " + ipfsexe.FullName);
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
                
                if (error.Contains("prometheus collector", StringComparison.OrdinalIgnoreCase)) //Hack: When a repo is being initialised via the --init parameter, it spits out these ignorable error messages.
                    return;

                if (error.Contains("mbinding.go:", StringComparison.OrdinalIgnoreCase)) //Hack: When a repo is being initialised via the --init parameter, it spits out these ignorable error messages.
                    return;

                if (!stoplogging && !string.IsNullOrEmpty(error))
                    L.Error(error);
            };

            if (RedirectRepository)
                dosContext.Environment.Add("IPFS_PATH", Instance.RepoDirectory.FullName);

            // if (_dispatcher != null)
            //    _dispatcher.Invoke(() => BindExit(dosContext));
            //else

            //BindExit(dosContext);
            
            return new ExecuteDos().CmdAsync(dosContext);
        }
    }
}