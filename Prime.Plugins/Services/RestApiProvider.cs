using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using plugins;
using Prime.Common;
using Prime.Plugins.Services.BitMex;
using RestEase;
using RestEase.Implementation;

namespace Prime.Plugins.Services
{
    public class RestApiClientProvider<T> where T : class
    {
        private readonly string _apiUrl;
        private readonly INetworkProvider _provider;
        private readonly Func<ApiKey, RequestModifier> _requestModifier;
        
        public JsonSerializerSettings JsonSerializerSettings { get; set; }
        public DecompressionMethods DecompressionMethods { get; set; } = DecompressionMethods.None;

        /// <summary>
        /// Creates instance that supports private API operations.
        /// </summary>
        /// <param name="apiUrl">API base URL.</param>
        /// <param name="networkProvider">Network provider that contains private user information.</param>
        /// <param name="requestModifier">Method-modifier that is called before request sending, usually implements authentication.</param>
        public RestApiClientProvider(string apiUrl, INetworkProvider networkProvider, Func<ApiKey, RequestModifier> requestModifier = null) : this(apiUrl)
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

        public T GetApi(NetworkProviderContext context = null)
        {
            return CreateClient().For<T>() as T;
        }

        public T GetApi(NetworkProviderPrivateContext context)
        {
            if(_requestModifier == null)
                throw new InvalidOperationException("Unable to get api because public constructor was used to create instance of RestApiClientProvider");

            var key = context.GetKey(_provider);

            return CreateClient(key).For<T>();
        }

        private RestClient CreateClient(ApiKey key = null)
        {
            var handler = key != null ? new ModifyingClientHttpHandler(_requestModifier.Invoke(key)) : new HttpClientHandler();

            handler.AutomaticDecompression = DecompressionMethods;

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_apiUrl)
            };

            var client = new RestClient(httpClient);

            if (JsonSerializerSettings != null)
                client.JsonSerializerSettings = JsonSerializerSettings;

            return client;
        }
    }
}
