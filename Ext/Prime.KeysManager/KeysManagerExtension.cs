using System;
using Prime.Base;
using Prime.Core;

namespace Prime.KeysManager
{
    public class KeysManagerExtension : IExtensionExecute
    {
        private static readonly ObjectId _id = "prime:KeysManagerExtension".GetObjectIdHashCode();
        public ObjectId Id => _id;

        public string Title { get; } = "Prime Key Manager Server";

        public Version Version { get; } = new Version("1.0.0");

        internal KeyManagerServer KeyManagerServerInstance;

        public void Main(ServerContext context)
        {
            KeyManagerServerInstance = new KeyManagerServer(context);
            KeyManagerServerInstance.Run();
        }
    }
}