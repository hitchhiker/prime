using System;
using LiteDB;

namespace Prime.Core
{
    public class PrimePackageInfo : IPrime
    {
        private static readonly ObjectId _id = "prime:core".GetObjectIdHashCode();
        public ObjectId Id => _id;

        public string Title { get; } = "Prime";

        public Version Version { get; } = new Version("1.0.0");
    }
}