using Prime.Base;

namespace Prime.PackageManager
{
    public class CataloguePublisherConfig
    {
        public ObjectId CatalogueId { get; set; }

        public string IPNSKey { get; set; }

        public string PubKey { get; set; }

        public string PriKey { get; set; }
    }
}