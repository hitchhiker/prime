using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using LiteDB;
using Prime.Base;
using Prime.Core;

namespace Prime.Finance.Services
{
    [Export(typeof(IExtension))]
    public class ExtensionInfo : IExtension
    {
        private static readonly ObjectId _id = "prime:finance-services".GetObjectIdHashCode();
        public ObjectId Id => _id;

        public string Title { get; } = "Prime Finance Services";

        public Version Version { get; } = new Version("1.0.0");
    }
}
