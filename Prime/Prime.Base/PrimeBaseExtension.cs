using System;
using System.Composition;
using System.Reflection;
using Prime.Base;
using Prime.Core;

namespace Prime.Base
{
    [Export(typeof(IExtension))]
    public class PrimeBaseExtension : IExtension
    {
        private static readonly ObjectId _id = "prime:base".GetObjectIdHashCode();
        public ObjectId Id => _id;

        public string Title { get; } = "Prime Base";

        public Version Version { get; } = new Version("1.0.2");
    }
}