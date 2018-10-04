using System.Collections.Generic;
using Newtonsoft.Json;
using Prime.Core;

namespace Prime.Radiant
{
    public class PackageConfig : JsonConfigBase<PackageConfig>
    {
        [JsonProperty("packages")]
        public List<PackageConfigItem> Packages { get; set; } = new List<PackageConfigItem>();
    }
}