using GalaSoft.MvvmLight.Messaging;
using Nito.AsyncEx;
using Prime.Base;
using Prime.Core;

namespace Prime.IPFS.Messaging {

    public class IpfsMessenger : CommonBase
    {
        internal readonly IpfsInstance IpfsInstance;
        internal readonly ContentHelper ContentHelper;

        public IpfsMessenger(IpfsInstance ipfsInstance) : base(ipfsInstance.Context.PrimeContext)
        {
            IpfsInstance = ipfsInstance;
            ContentHelper = new ContentHelper(this);
        }

        public void Start()
        {
            M.RegisterAsync<IpfsStartRequest>(this, IpfsStartRequest);
            M.RegisterAsync<IpfsStopRequest>(this, IpfsStopRequest);
            M.RegisterAsync<IpfsStatusRequest>(this, m=> SendIpfsStatus());
            M.RegisterAsync<IpfsVersionRequest>(this, m=> SendIpfsVersion());
            ContentHelper.Register();
        }

        private void IpfsStartRequest(IpfsStartRequest m)
        {
            IpfsInstance.StartDaemon();
        }

        private void IpfsStopRequest(IpfsStopRequest m)
        {
            IpfsInstance.StopDaemon();
        }

        private void SendIpfsVersion()
        {
            if (IpfsInstance.Daemon.CurrentState == DaemonState.Running)
            {
                AsyncContext.Run(async () =>
                {
                    var v = await IpfsInstance.Client.VersionAsync();
                    M.SendAsync(new IpfsVersionResponse() { Version = v.Get("Version")/*, Item = v*/ });
                });
            }
        }

        public void SendIpfsStatus()
        {
            M.SendAsync(new IpfsStatusMessage(IpfsInstance.Daemon.State().GetServiceStatus()));
        }

        public void Stop()
        {
            M.UnregisterAsync(this);
        }
    
    }
}
