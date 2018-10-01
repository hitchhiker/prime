﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Base.Messaging.Common;
using Prime.Core;
using Prime.PackageManager;

namespace Prime.Core
{
    [Export(typeof(IExtension))]
    public class ExtensionInfo : IExtensionInitPrimeInstance
    {
        private static readonly ObjectId _id = "prime:pm".GetObjectIdHashCode();
        public ObjectId Id => _id;
        public string Title => "Prime Package Manager";
        public Version Version => new Version("1.0.2");

        public void Init(PrimeInstance instance)
        {
            instance.M.RegisterAsync(this, (PrimePackagesRequest m) =>
            {
                PackageCatalogueEntry.Request(instance, m.Options);
                instance.M.Send(new PrimePackagesResponse(m) {Success = true});
            });
        }

        public static void DummyRef()
        {
            Noop(typeof(JsonReaderException));
        }

        private static void Noop(Type _) { }
    }
}