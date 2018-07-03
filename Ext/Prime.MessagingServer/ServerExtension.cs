using System;
using System.Composition;
using Prime.Base;
using Prime.Core;

namespace Prime.MessagingServer
{
    [Export(typeof(IExtension))]
    public class ServerExtension : IExtensionExecute, IExtensionStartup
    {
        private static readonly ObjectId _id = "prime:messageserver".GetObjectIdHashCode();
        public ObjectId Id => _id;

        public string Title { get; } = "Prime Message Server";

        public Version Version { get; } = new Version("1.0.0");

        internal Server MessageServerInstance;

        public void Main(ServerContext context)
        {
            MessageServerInstance = new Server(context);
        }

        public void PrimeStarted()
        {
            MessageServerInstance.Start();
        }
    }
}
