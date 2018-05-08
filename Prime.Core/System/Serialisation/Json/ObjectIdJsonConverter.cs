using System;
using LiteDB;
using Newtonsoft.Json;

namespace Prime.Core
{
    public class ObjectIdJsonConverter : JsonConverter
    {
        private static readonly Type ObjectIdType = typeof(ObjectId);

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var v = (string) reader.Value;
            return new ObjectId(v);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == ObjectIdType;
        }
    }
}