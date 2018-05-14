using System.Threading;
using Prime.Core;

namespace Prime.Console.Frank.Ipfs
{
    public class IpfsMessageTest : TestServerBase
    {
        public IpfsMessageTest(ServerContext server) : base(server)
        {
        }

        public override void Go()
        {
            var prime = new Core.Prime(S);

            var e = prime.Extensions;

            e.Load<IExtensionExecute>(KnownExtensionsManifest.Ipfs);
            
            var versionOk = false;
            var statusStarted = false;
            var statusStopped = false;

            M.RegisterAsync<IpfsVersionResponse>(this, m =>
            {
                L.Log(m.Version);
                versionOk = true;
            });

            M.RegisterAsync<IpfsStatusMessage>(this, m =>
            {
                L.Log(m.ServiceStatus.ToString());
                statusStarted = statusStarted || m.ServiceStatus == ServiceStatus.Running;
                statusStopped = statusStarted && m.ServiceStatus == ServiceStatus.Stopped;

                if (m.ServiceStatus == ServiceStatus.Running)
                    M.SendAsync(new IpfsStopRequest());
            });

            M.SendAsync(new IpfsStartRequest());
            M.SendAsync(new IpfsVersionRequest());

            do
            {
                Thread.Sleep(1);
            } while (!versionOk && !statusStarted && !statusStopped);
        }
    }
}