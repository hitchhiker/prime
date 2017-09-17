using System;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Utility
{
    public class Logging
    {
        private Logging()
        {
            DefaultLogger = new MessengerLogger();
        }

        public static Logging I => Lazy.Value;
        private static readonly Lazy<Logging> Lazy = new Lazy<Logging>(()=>new Logging());

        public ILogger DefaultLogger { get; }
    }
}