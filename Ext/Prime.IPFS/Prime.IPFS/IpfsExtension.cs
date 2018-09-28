using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Prime.Base;
using Prime.Base.Messaging.Common;
using Prime.Core;
using Prime.IPFS.Messaging;

namespace Prime.IPFS
{
    public abstract class IpfsExtensionBase : IExtensionExecute, IExtensionPlatform, IExtensionGracefullShutdown
    {
        private static readonly ObjectId _id = new ObjectId("3575ddcb0d8647d75fbf044c"); // "prime:ipfs";

        public ObjectId Id => _id;

        internal IpfsInstance IpfsInstance { get; private set; }

        public void Main(PrimeContext context)
        {
            var ctx = new IpfsInstanceContext(context, GetPlatformBase());
            IpfsInstance = new IpfsInstance(ctx);

            void Shutdown(PrimeShutdownNow obj)
            {
                if (IpfsInstance.Daemon.CurrentState == DaemonState.System)
                    return;

                context.M.Send(new IpfsStopRequest());
                do
                {
                    Thread.Sleep(1);
                } while (IpfsInstance.Daemon.CurrentState == DaemonState.Stopped);
                HasShutdown = true;
            }

            context.M.Register<PrimeShutdownNow>(this, Shutdown);
        }

        public string Title => "Prime Ipfs Go";

        public Version Version => new Version(1, 3, 0);

        public abstract IpfsPlatformBase GetPlatformBase();

        public abstract Platform Platform { get; }

        public bool HasShutdown { get; private set; }
    }
}
