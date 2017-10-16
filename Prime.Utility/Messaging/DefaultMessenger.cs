using System;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Utility
{
    public class DefaultMessenger
    {
        private DefaultMessenger()
        {
            Default = Messenger.Default;
        }

        public static DefaultMessenger I => Lazy.Value;
        private static readonly Lazy<DefaultMessenger> Lazy = new Lazy<DefaultMessenger>(()=>new DefaultMessenger());

        public IMessenger Default;

        public readonly object Token = new object();
    }
}