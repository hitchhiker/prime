using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Prime.Plugins.Services.Kraken.Converters
{
    internal class OhlcJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var jErrors = jObject["error"];
            var jResults = jObject["result"]?.Value<JObject>();

            var rOhlc = new KrakenSchema.OhlcResponse
            {
                error = jErrors.Select(x => x.Value<String>()).ToArray(),
                result = new KrakenSchema.OhlcResultResponse()
            };

            if (jResults != null)
            {
                rOhlc.result.pairs = new Dictionary<string, KrakenSchema.OhlcDataRespose[]>();

                foreach (var jResult in jResults)
                {
                    if (jResult.Key.Equals("last"))
                    {
                        rOhlc.result.last = jResult.Value.Value<long>();
                    }
                    else
                    {
                        var ohlcData = new List<KrakenSchema.OhlcDataRespose>();

                        var rawData = jResult.Value.ToObject<decimal[][]>();

                        foreach (var data in rawData)
                        {
                            ohlcData.Add(new KrakenSchema.OhlcDataRespose()
                            {
                                time = data[0],
                                open = data[1],
                                high = data[2],
                                low = data[3],
                                close = data[4],
                                vwap = data[5],
                                volume = data[6],
                                count = data[7]
                            });
                        }

                        rOhlc.result.pairs.Add(jResult.Key, ohlcData.ToArray());
                    }
                }
            }

            return rOhlc;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(KrakenSchema.OhlcResponse).IsAssignableFrom(objectType);
        }
    }
}
