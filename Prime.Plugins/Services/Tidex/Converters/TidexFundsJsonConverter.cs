using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Prime.Plugins.Services.Tidex.Converters
{
    [Obsolete("Was supposed to be used. Left as an example")]
    internal class TidexFundsJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            var json = jObject.Root.ToString();
            var autoObject = JsonConvert.DeserializeObject<TidexSchema.UserInfoExtResponse>(json);
            
            var jReturn = jObject["return"];
            var jFunds = jReturn["funds"].Value<JObject>();

            foreach (var jFund in jFunds)
            {
                var currency = jFund.Key;
                var value = jFund.Value["value"].Value<decimal>();
                var inOrders = jFund.Value["inOrders"].Value<decimal>();
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(TidexSchema.UserInfoExtResponse).IsAssignableFrom(objectType);
        }
    }
}
