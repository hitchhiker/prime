using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.OkCoin
{
    public class OkCoinAuthenticator : BaseAuthenticator
    {
        private string publicKey = "7a7bd50a-2f57-4c60-940b-db6fc40b26dd";
        private string secret = "00D4A2A4A1DC49F664B9B2DBDE4C10D8";

        public OkCoinAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var parameters = request.Content?.ReadAsStringAsync()?.Result;

            if (parameters == null)
            {
                parameters = request.RequestUri.Query.TrimStart('?');
            }

            string[] arrParameters;

            if (string.IsNullOrWhiteSpace(parameters))
            {
                arrParameters = $"api_key={/*ApiKey.Key*/publicKey}".Split('&');
            }
            else
            {
                arrParameters = $"{parameters}&api_key={/*ApiKey.Key*/publicKey}".Split('&');
            }

            Array.Sort(arrParameters); //Sorts array alphabetically.

            string strToHash = string.Join("&", arrParameters);
            strToHash = $"{strToHash}&secret_key={/*ApiKey.Secret*/secret}";

            var signature = HashMD5Hex(strToHash).ToUpper();

            request.Content = new StringContent($"{strToHash}&sign={signature}", Encoding.UTF8, "application/x-www-form-urlencoded");
        }

    }


}
