using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prime.Base.DStore;
using Prime.Core;

namespace Prime.PackageManager
{
    public class PackageInstance
    {
        public PackageInstance() { }

        public PackageInstance(PackageMeta meta)
        {
            Platform = meta.Platform;
            Version = meta.Version;
        }

        [JsonProperty("platform", DefaultValueHandling = DefaultValueHandling.Ignore), JsonConverter(typeof(StringEnumConverter)), DefaultValue(Platform.NotSpecified)]
        public Platform Platform { get; set; } = Platform.NotSpecified;

        [JsonProperty("version"), JsonConverter(typeof(VersionJsonConverter))]
        public Version Version { get; set; }
        
        [JsonProperty("curi", NullValueHandling = NullValueHandling.Ignore)]
        public ContentUri ContentUri { get; set; }
    }
}