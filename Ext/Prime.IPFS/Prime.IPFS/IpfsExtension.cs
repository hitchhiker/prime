using System;
using System.Collections.Generic;
using System.Text;
using Prime.Base;
using Prime.Core;

namespace Prime.IPFS
{
    public abstract class IpfsExtension : IExtensionExecute, IExtensionPlatform
    {
        private static readonly ObjectId _id = new ObjectId("3575ddcb0d8647d75fbf044c"); // "prime:ipfs";

        public ObjectId Id => _id;

        public IpfsMessenger Messenger;

        public void Main(ServerContext context)
        {
            var ctx = new IpfsInstanceContext(context, GetPlatformBase());
            var ipfs = new IpfsInstance(ctx);
            Messenger = new IpfsMessenger(context, ipfs);
        }

        public string Title => "Prime Ipfs Go";

        public Version Version => new Version(1, 3, 0);

        public abstract IpfsPlatformBase GetPlatformBase();

        public abstract Platform Platform { get; }
    }
}
