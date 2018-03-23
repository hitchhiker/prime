﻿using System;
using System.Net.Http;
using System.Threading;
using Prime.Common;

namespace Prime.Plugins.Services.Poloniex
{
    public class PoloniexAuthenticator : BaseAuthenticator
    {
        public PoloniexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModifyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;
            var postData = request.Content.ReadAsStringAsync().Result;
            var sign = HashHMACSHA512Hex(postData, ApiKey.Secret);

            headers.Add("Key", ApiKey.Key);
            headers.Add("Sign", sign);
        }
    }
}
