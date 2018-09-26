using System;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    public class DefaultMessenger
    {
        private DefaultMessenger()
        {
            Instance = new Messenger() {FriendlyName = "Default messenger"};
        }

        public static DefaultMessenger I => Lazy.Value;
        private static readonly Lazy<DefaultMessenger> Lazy = new Lazy<DefaultMessenger>(() => new DefaultMessenger());

        public IMessenger Instance;

        public readonly object Token = new object();
    }
}