using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// Creates instance that supports private API operations.
        /// </summary>
        /// <param name="apiUrl">API base URL.</param>
        /// <param name="networkProvider">Network provider that contains private user information.</param>
        /// <param name="requestModifier">Method-modifier that is called before request sending, usually implements authentication.</param>
        public RestApiClientProvider(string apiUrl, INetworkProvider networkProvider, Func<ApiKey, RequestModifier> requestModifier) : this(apiUrl)
        {
            _provider = networkProvider;
            _requestModifier = requestModifier;
        }

        /// <summary>
        /// Creates instance that supports only public API operations.
        /// </summary>
        /// <param name="apiUrl">API base URL.</param>
        public RestApiClientProvider(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public T GetApi(NetworkProviderContext context)
        {
            return new RestClient(_apiUrl)
            {
                JsonSerializerSettings = JsonSerializerSettings
            }.For<T>() as T;
        }

        public T GetApi(NetworkProviderPrivateContext context)
        {
            if(_requestModifier == null)
                throw new InvalidOperationException("Operation is not supported, please use full constructor");

            var key = context.GetKey(_provider);

            return new RestClient(_apiUrl, _requestModifier.Invoke(key))
            {
                JsonSerializerSettings = JsonSerializerSettings
                // ResponseDeserializer = new DebugDeserialiser()
            }.For<T>();
        }
    }
}
