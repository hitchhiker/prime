using System;
using Newtonsoft.Json;

namespace plugins
{
    public class HistoryLine : IEquatable<HistoryLine>
    {
        [JsonProperty(PropertyName = "time")]
        public DateTime UtcDate { get; set; }

        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }

        [JsonProperty(PropertyName = "trade_id")]
        public string RemoteId { get; set; }

        [JsonProperty(PropertyName = "size")]
        public int Size { get; set; }

        public bool Equals(HistoryLine other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || string.Equals(RemoteId, other.RemoteId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HistoryLine) obj);
        }

        public override int GetHashCode()
        {
            return RemoteId?.GetHashCode() ?? 0;
        }
    }
}