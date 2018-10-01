using System;
using System.Composition;
using LiteDB;
using Prime.Base;

namespace Prime.Core
{
    [Export(typeof(IExtension))]
    public class PrimeCoreExtension : IPrime
    {
        private static readonly ObjectId _id = "prime:core".GetObjectIdHashCode();
        public ObjectId Id => _id;

        public string Title { get; } = "Prime";

        public Version Version { get; } = new Version("1.0.4");
    }
}