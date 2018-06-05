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
                arrParameters = $"api_key={ApiKey.Key}".Split('&');
            }
            else
            {
                arrParameters = $"{parameters}&api_key={ApiKey.Key}".Split('&');
            }

            Array.Sort(arrParameters); //Sorts array alphabetically.

            string strToHash = string.Join("&", arrParameters);
            strToHash = $"{strToHash}&secret_key={ApiKey.Secret}";

            var signature = HashMD5Hex(strToHash).ToUpper();

            request.Content = new StringContent($"{strToHash}&sign={signature}", Encoding.UTF8, "application/x-www-form-urlencoded");
        }

    }


}
