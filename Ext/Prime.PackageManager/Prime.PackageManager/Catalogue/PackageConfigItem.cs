using Newtonsoft.Json;

namespace Prime.Radiant
{
    public class PackageConfigItem
    {
        [JsonProperty("src_csproj")]
        public string SourceProject { get; set; }
    }
}