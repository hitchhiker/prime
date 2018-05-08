using System;
using Newtonsoft.Json;

namespace Prime.Core
{
    public class VersionJsonConverter : JsonConverter
    {
        private static readonly Type Type = typeof(Version);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var v = (string)reader.Value;
            return new Version(v);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == Type;
        }
    }
}