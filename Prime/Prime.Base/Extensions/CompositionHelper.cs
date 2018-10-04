using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prime.Base;
using Prime.Core;

namespace Prime.Base
{
    public class CompositionHelper : IExtension
    {
        [JsonProperty("Id"), JsonConverter(typeof(ObjectIdJsonConverter))]
        public ObjectId Id { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Version"), JsonConverter(typeof(VersionJsonConverter))]
        public Version Version { get; set; }

        [JsonProperty("platform", DefaultValueHandling = DefaultValueHandling.Ignore), JsonConverter(typeof(StringEnumConverter)), DefaultValue(Platform.NotSpecified)]
        public Platform Platform { get; set; } = Platform.NotSpecified;

        public static string Serialised(IExtension ext)
        {
            if (ext == null || ext.Id.IsNullOrEmpty())
                return null;

            var o = new CompositionHelper {Id = ext.Id, Version = ext.Version, Title = ext.Title};

            if (ext is IExtensionPlatform plat)
                o.Platform = plat.Platform;

            return JsonConvert.SerializeObject(o);
        }
    }
}