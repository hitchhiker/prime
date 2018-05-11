using System;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    public class DefaultMessenger
    {
        private DefaultMessenger()
        {
            DefaultServer = new Messenger() {FriendlyName = "Server default messenger"};
            DefaultClient = new Messenger() {FriendlyName = "Client default messenger"};
        }

        public static DefaultMessenger I => Lazy.Value;
        private static readonly Lazy<DefaultMessenger> Lazy = new Lazy<DefaultMessenger>(() => new DefaultMessenger());

        public IMessenger DefaultServer;
        public IMessenger DefaultClient;

        public readonly object Token = new object();
    }
}