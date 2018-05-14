using System;
using Prime.Base;
using Prime.Core;
using Prime.Manager;

namespace Prime.KeysManager
{
    public class ManagerServiceExtension : IExtensionExecute
    {
        private static readonly ObjectId _id = "prime:managerservice".GetObjectIdHashCode();
        public ObjectId Id => _id;

        public string Title { get; } = "Prime Key Manager Server";

        public Version Version { get; } = new Version("1.0.0");

        internal ManagerService ManagerServiceInstance;

        public void Main(ServerContext context)
        {
            ManagerServiceInstance = new ManagerService(context);
            ManagerServiceInstance.Init();
        }
    }
}