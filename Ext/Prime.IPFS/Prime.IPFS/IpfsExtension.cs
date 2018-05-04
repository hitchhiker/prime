using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;
using Prime.Core;

namespace Prime.IPFS
{
    public abstract class IpfsExtension : IExtensionExecute
    {
        private static readonly ObjectId _id = new ObjectId("3575ddcb0d8647d75fbf044c"); // "prime:ipfs";

        public ObjectId Id => _id;

        public void Main(Core.AppContext context)
        {
            var ctx = new IpfsInstanceContext(null, null);
            var ipfs = new IpfsInstance(ctx);
            var messenger = new IpfsMessenger(context, ipfs);
        }

        public string Title => "Ipfs";

        public Version Version => new Version(1, 3, 0);

        public abstract Platform Platform { get; }
    }
}
