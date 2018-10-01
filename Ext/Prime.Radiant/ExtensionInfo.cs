using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Base.Messaging.Common;
using Prime.Core;

namespace Prime.Radiant
{
    [Export(typeof(IExtension))]
    public class ExtensionInfo : IExtensionInitPrimeInstance
    {
        private static readonly ObjectId _id = "prime:radiant".GetObjectIdHashCode();
        public ObjectId Id => _id;
        public string Title => "Prime Radiant Catalogue Manager";
        public Version Version => new Version("1.0.4");

        public void Init(PrimeInstance instance)
        {
            instance.M.RegisterAsync<PrimePublishRequest>(this, m => PublishCatalogue(m, instance));
            instance.M.RegisterAsync<PrimeUpdateRequest>(this, m => UpdateCatalogue(m, instance));
        }

        private void PublishCatalogue(PrimePublishRequest m, PrimeInstance instance)
        {
            CatalogueBootEntry.Publish(instance, m.PublisherConfigPath);
            instance.M.Send(new PrimePublishResponse(m) {Success = true});
        }

        private void UpdateCatalogue(PrimeUpdateRequest m, PrimeInstance instance)
        {
            CatalogueBootEntry.Update(instance);
            instance.M.Send(new PrimeUpdateResponse(m) { Success = true });
        }

        public static void DummyRef()
        {
            Noop(typeof(JsonReaderException));
        }

        private static void Noop(Type _) { }
    }
}
