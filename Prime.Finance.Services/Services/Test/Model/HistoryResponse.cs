using System.Collections.Generic;
using Newtonsoft.Json;

namespace Prime.Finance.Services.Services.Test.Model
{
    public class HistoryResponse
    {
        [JsonProperty(PropertyName = "history")]
        public List<HistoryLine> Items { get; set; }

    }
}