using System;
using System.IO;
using System.Reflection;

namespace Prime.Utility
{
    public class GlobalMisc
    {
        private GlobalMisc() {}

        public static GlobalMisc I => Lazy.Value;
        private static readonly Lazy<GlobalMisc> Lazy = new Lazy<GlobalMisc>(()=>new GlobalMisc());

        public Assembly MainAssembly { get; set; }

    }
}