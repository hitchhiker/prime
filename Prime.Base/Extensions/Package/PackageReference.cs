using System;
using System.Reflection;
using Newtonsoft.Json;
using Prime.Base;

namespace Prime.Core
{
    public class PackageReference
    {
        public PackageReference() { }

        public PackageReference(IExtension ext)
        {
            Id = ext.Id;
            Version = ext.Version;
        }

        [JsonProperty("id"), JsonConverter(typeof(ObjectIdJsonConverter))]
        public ObjectId Id { get; set; }

        [JsonProperty("version"), JsonConverter(typeof(VersionJsonConverter))]
        public Version Version { get; set; }
        
        [JsonIgnore]
        public Assembly Assembly { get; set; }
    }
}