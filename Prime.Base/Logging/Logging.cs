using System;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    public class Logging
    {
        private Logging()
        {
            DefaultLogger = new NullLogger();
        }

        public static Logging I => Lazy.Value;
        private static readonly Lazy<Logging> Lazy = new Lazy<Logging>(()=>new Logging());

        public ILogger DefaultLogger { get; }
    }
}