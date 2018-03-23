using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prime.Common;

namespace Prime.Plugins.Services.Bitfinex
{
    internal class BitfinexAuthenticator : BaseAuthenticator
    {
        public BitfinexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var bodyPrevJson = request.Content.ReadAsStringAsync().Result;
            var bodyPrev = JsonConvert.DeserializeObject<object>(bodyPrevJson) as JToken;
            var jObject = bodyPrev.Value<JObject>();

            var className = jObject["ClassName"].Value<string>();

            var requestObj = DeserializeRequest(bodyPrevJson, className);

            if (!(requestObj is BitfinexSchema.BaseRequest requestBaseObj))
                throw new FormatException("Incorrect format of request");

            var nonce = GetUnixEpochNonce();

            requestBaseObj.request = request.RequestUri.AbsolutePath;
            requestBaseObj.nonce = nonce.ToString();

            var bodyJson = JsonConvert.SerializeObject(requestObj);
            var payload = ToBase64(bodyJson);
            var signature = HashHMACSHA384Hex(payload, ApiKey.Secret);

            request.Content = new StringContent(bodyJson);

            request.Headers.Add("X-BFX-APIKEY", ApiKey.Key);
            request.Headers.Add("X-BFX-PAYLOAD", payload);
            request.Headers.Add("X-BFX-SIGNATURE", signature);
        }

        private object DeserializeRequest(string json, string className)
        {
            switch (className)
            {
                case "NewOrderRequest":
                    return JsonConvert.DeserializeObject<BitfinexSchema.NewOrderRequest>(json);
                case "WalletBalancesRequest":
                    return JsonConvert.DeserializeObject<BitfinexSchema.WalletBalancesRequest>(json);
                case "AccountInfoRequest":
                    return JsonConvert.DeserializeObject<BitfinexSchema.AccountInfoRequest>(json);
                case "WithdrawalRequest":
                    return JsonConvert.DeserializeObject<BitfinexSchema.WithdrawalRequest>(json);
                case "OrderStatusRequest":
                    return JsonConvert.DeserializeObject<BitfinexSchema.OrderStatusRequest>(json);
                case "ActiveOrdersRequest":
                    return JsonConvert.DeserializeObject<BitfinexSchema.ActiveOrdersRequest>(json);
                case "OrdersHistoryRequest":
                    return JsonConvert.DeserializeObject<BitfinexSchema.OrdersHistoryRequest>(json);
                default:
                    throw new NotSupportedException("Deserialization of specified class is not supported");
            }
        }
    }
}
