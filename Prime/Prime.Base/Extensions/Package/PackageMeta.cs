using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prime.Base;

namespace Prime.Core
{
    public class PackageMeta : IExtensionPlatform
    {
        public static string CommonName = "prime-ext.json";

        public PackageMeta() { }

        public PackageMeta(IExtension ext, Version assemblyVersion = null)
        {
            Title = ext.Title;
            Id = ext.Id;

            Version = assemblyVersion ?? ext.Version;

            if (ext is IExtensionPlatform plat)
                Platform = plat.Platform;
            else if (ext is CompositionHelper helper)
                Platform = helper.Platform;
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
        public List<PackageReference> PackageReferences { get; set; } = new List<PackageReference>();

        public string ToJsonSimple()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static PackageMeta From(DirectoryInfo dir)
        {
            var fi = new FileInfo(Path.Combine(dir.FullName, CommonName));
            return From(fi);
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
            try
            {
                return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<PackageMeta>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}