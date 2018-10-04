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
        public static readonly ObjectId StaticId = "prime:base".GetObjectIdHashCode();
        public ObjectId Id => StaticId;

        public string Title { get; } = "Prime Base";

        public Version Version { get; } = new Version("1.0.5");
    }
}