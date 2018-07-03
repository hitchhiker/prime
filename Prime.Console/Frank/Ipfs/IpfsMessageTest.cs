using System.Threading;
using Prime.Core;

namespace Prime.Console.Frank.Ipfs
{
    public class IpfsMessageTest : TestServerBase
    {
        public IpfsMessageTest(Core.PrimeInstance server) : base(server)
        {
        }

        public override void Go()
        {
            var stopSent = false;
            var versionSent = false;
            var versionFound = false;
            var statusStartedFound = false;
            var statusStoppedFound = false;

            M.RegisterAsync<IpfsVersionResponse>(this, m =>
            {
                L.Log("IPFS Version: " + m.Version);
                versionFound = true;
            });

            M.RegisterAsync<IpfsStatusMessage>(this, m =>
            {
                L.Log(m.ServiceStatus.ToString());
                statusStartedFound = statusStartedFound || m.ServiceStatus == ServiceStatus.Running;
                statusStoppedFound = statusStoppedFound || (statusStartedFound && m.ServiceStatus == ServiceStatus.Stopped);
            });

            M.SendAsync(new IpfsStartRequest());

            do
            {
                Thread.Sleep(10);

                if (statusStartedFound && !versionSent)
                {
                    versionSent = true;
                    M.SendAsync(new IpfsVersionRequest());
                }

                if (versionFound && !stopSent)
                {
                    stopSent = true;
                    Thread.Sleep(30000);
                    M.SendAsync(new IpfsStopRequest());
                }

            } while (!versionFound || !statusStartedFound || !statusStoppedFound);

            L.Log("IPFS Test complete.");
        }
    }
}