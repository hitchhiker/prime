using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Plugins.Services.BitMex;
using RestEase;

namespace Prime.Plugins.Services
{
    public class RestApiClientProvider<T> where T : class
    {
        private readonly string _apiUrl;
        private readonly INetworkProvider _provider;
        private readonly Func<ApiKey, RequestModifier> _requestModifier;

        public RestApiClientProvider(string apiUrl, INetworkProvider networkProvider, Func<ApiKey, RequestModifier> requestModifier)
        {
            _apiUrl = apiUrl;
            _provider = networkProvider;
            _requestModifier = requestModifier;
        }

        public static RestApiClientProvider<T> Create(string apiUrl, INetworkProvider networkProvider, Func<ApiKey, RequestModifier> requestModifier)
        {
            return new RestApiClientProvider<T>(apiUrl, networkProvider, requestModifier);
        }

        public T GetApi(NetworkProviderContext context)
        {
            return RestClient.For<T>(_apiUrl) as T;
        }

        public T GetApi(NetworkProviderPrivateContext context)
        {
            var key = context.GetKey(_provider);

            return RestClient.For<T>(_apiUrl, _requestModifier.Invoke(key)) as T;
        }
    }
}
