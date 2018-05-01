namespace Prime.IPFS
{
    public abstract class IpfsDaemonBase {

        public readonly IpfsInstance Instance;

        protected IpfsDaemonBase(IpfsInstance instance)
        {
            Instance = instance;
        }

        public abstract IpFsDaemonState State();

        public abstract void Start();

        public abstract void Stop();
    }
}