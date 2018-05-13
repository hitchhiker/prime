using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prime.Base;

namespace Prime.Core
{
    public class PackageMeta : IExtensionPlatform
    {
        public PackageMeta() { }

        public PackageMeta(IExtension ext)
        {
            Title = ext.Title;
            Id = ext.Id;

            Version = ext.Version;

            if (ext is IExtensionPlatform plat)
                Platform = plat.Platform;
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("id"), JsonConverter(typeof(ObjectIdJsonConverter))]
        public ObjectId Id { get; set; }

        [JsonProperty("platform", DefaultValueHandling = DefaultValueHandling.Ignore), JsonConverter(typeof(StringEnumConverter)), DefaultValue(Platform.NotSpecified)]
        public Platform Platform { get; set; } = Platform.NotSpecified;

        [JsonProperty("version"), JsonConverter(typeof(VersionJsonConverter))]
        public Version Version { get; set; }

        [JsonProperty("extReferences")]
        public List<ObjectId> ExtensionReferences { get; set; } = new List<ObjectId>();

        public string ToJsonSimple()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static PackageMeta From(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
                return null;

            var txt = File.ReadAllText(fileInfo.FullName);
            return From(txt);
        }

        public static PackageMeta From(string json)
        {
            return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<PackageMeta>(json);
        }
    }
}