using System;
using Prime.Base;
using Prime.Core;

namespace Prime.MessagingServer
{
    public class ExtensionEntry : IExtensionExecute
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
    }
}
