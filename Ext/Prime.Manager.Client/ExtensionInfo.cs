using System;
using Prime.Base;
using Prime.Core;

namespace Prime.KeysManager
{
    public class ExtensionInfo : IExtension
    {
        private static readonly ObjectId _id = "prime:manager-client".GetObjectIdHashCode();
        public ObjectId Id => _id;

        public string Title { get; } = "Prime Manager Client";

        public Version Version { get; } = new Version("1.0.0");
    }
}