using System.Collections.Generic;
using Newtonsoft.Json;

namespace plugins
{
    public class HistoryResponse
    {
        [JsonProperty(PropertyName = "history")]
        public List<HistoryLine> Items { get; set; }

    }
}