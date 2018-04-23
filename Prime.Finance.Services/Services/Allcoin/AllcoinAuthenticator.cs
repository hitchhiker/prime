using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Common;

namespace Prime.Finance.Services.Services.Allcoin
{
    public class AllcoinAuthenticator : BaseAuthenticator
    {

        public AllcoinAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var parameters = request.Content?.ReadAsStringAsync()?.Result;
            string[] arrParameters;

            //Arrange the parameters in ascending alphabetical order
            if (string.IsNullOrWhiteSpace(parameters))
            {
                arrParameters = new string[1];
                arrParameters[0] = $"api_key={ApiKey.Key}";
            }
            else
            {
                arrParameters = $"{parameters}&api_key={ApiKey.Key}".Split('&');
            }

            Array.Sort(arrParameters); //Sorts array alphabetically.

            var queryString = $"{string.Join("&", arrParameters)}&secret_key={ApiKey.Secret}";

            var signature = HashMD5Hex(queryString).ToUpper();

            request.Content = new StringContent($"{queryString}&sign={signature}", Encoding.UTF8, "application/x-www-form-urlencoded");
         }
    }
}
