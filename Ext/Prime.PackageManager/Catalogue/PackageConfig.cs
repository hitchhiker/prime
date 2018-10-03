using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prime.Base;
using Prime.Core;

namespace Prime.Radiant
{
    public class PackageConfig : JsonConfigBase<PackageConfig>
    {
        [JsonProperty("packages")]
        public List<PackageConfigItem> Packages { get; set; } = new List<PackageConfigItem>();
    }
}