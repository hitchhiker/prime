using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Prime.Core;
using Timer = System.Timers.Timer;

namespace Prime.IPFS
{
    public class IpfsGenericDaemon : IpfsDaemonBase
    {
        private readonly IpfsInstance _instance;
        private Timer _externalPollTimer;

        public bool RedirectRepository { get; set; } = true;

        public bool AutoRestart { get; set; } = true;

        public IpfsGenericDaemon(IpfsInstance instance) : base(instance)
        {
            _instance = instance;
            CurrentState = DaemonState.Stopped;
        }

        public ILogger L => _instance.L;

        private volatile bool _isStarted;

        public override DaemonState State()
        {
            return CurrentState;
        }

        public override void Start()
        {
            if (_isStarted && CurrentState != DaemonState.Stopped)
                return;

            _isStarted = true;
            StartInternal();
        }

        private void StartInternal()
        {
            if (Instance.IsIpfsExternalRunning())
            {
                InitForExternal();
                return;
            }
        }

        public override void Stop()
        {

        }

        private void InitForExternal()
        {
            L.Info("IPFS is already running on this machine, we're using that instance.");
            CurrentState = DaemonState.System;
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
                CurrentState = DaemonState.Stopped;
                return;
            }

            CurrentState = DaemonState.System;
            _externalPollTimer.Enabled = true;
        }
    }
}
