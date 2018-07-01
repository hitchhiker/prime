using System;
using System.Threading.Tasks;

namespace Prime.IPFS
{
    public abstract class IpfsDaemonBase {

        public readonly IpfsInstance Instance;

        protected IpfsDaemonBase(IpfsInstance instance)
        {
            Instance = instance;
        }

        public abstract DaemonState State();

        public abstract void Start();

        public abstract void Stop();

        public Action<DaemonState> StateChangedAction { protected get; set; }

        private DaemonState _currentState;
        public DaemonState CurrentState
        {
            get => _currentState;
            protected set
            {
                var c = _currentState != value;
                _currentState = value;
                if (c)
                    Task.Run(() => StateChangedAction?.Invoke(value));
            }
        }
    }
}