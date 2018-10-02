using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Core;

namespace Prime.ExtensionPackager
{
    public class ExtensionPackerExtensionInfo : IExtension
    {
        private static readonly ObjectId _id = "prime:extensionpacker".GetObjectIdHashCode();
        public ObjectId Id => _id;
        public string Title => "Prime Extension Packer Tool";
        public Version Version => new Version("1.0.5");

        public static void DummyRef()
        {
            Noop(typeof(JsonReaderException));
        }

        private static void Noop(Type _) { }
    }
}
