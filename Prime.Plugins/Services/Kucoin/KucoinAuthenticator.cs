using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Kucoin
{
    public class KucoinAuthenticator : BaseAuthenticator
    {

        public KucoinAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;
            var timeStamp = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.
            
            string endpoint = request.RequestUri.AbsolutePath;

            //Arrange the parameters in ascending alphabetical order (lower cases first), then combine them with & (don't urlencode them, don't add ?, don't add extra &), e.g. amount=10&price=1.1&type=BUY 
            string parameters = request.RequestUri.Query.Replace("?", "");
            string[] arrParameters = parameters.Split('&');

            Array.Sort(arrParameters); //Sorts array alphabetically.

            string queryString = string.Join("&", arrParameters);
            
            //splice string for signing
            string strForSign = endpoint + "/" + timeStamp + "/" + queryString;

            //Make a base64 encoding of the completed string
            string signatureStr = Base64Encode(strForSign);

            var signature = HashHMACSHA256Hex(signatureStr, ApiKey.Secret);

            headers.Add("KC-API-KEY", ApiKey.Key);
            headers.Add("KC-API-NONCE", timeStamp.ToString());
            headers.Add("KC-API-SIGNATURE", signature);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
