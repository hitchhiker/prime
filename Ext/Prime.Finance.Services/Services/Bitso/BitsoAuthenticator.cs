﻿using System;
using System.Net.Http;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Bitso
{
    public class BitsoAuthenticator : BaseAuthenticator
    {

        public BitsoAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var nonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var content = request.Content?.ReadAsStringAsync()?.Result;
            var headers = request.Headers;

            string path = request.RequestUri.AbsolutePath;
            
            var signature = HashHMACSHA256Hex($"{nonce}{request.Method}{path}{content}", ApiKey.Secret);

            headers.Add("Authorization", $"Bitso {ApiKey.Key}:{nonce}:{signature}");
        }
    }
}
