using RestSharp;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Prime.Core;
using Prime.Plugins.Services.BitStamp;

namespace Rokolab.BitstampClient
{
    public class RequestAuthenticator : IRequestAuthenticator
    {
        public static IExchangeProvider BitStampExchangeProvider => Networks.I.ExchangeProviders.FirstOrDefault(x=> x.GetType() == typeof(BitStampProvider));

        private readonly string _clientId;
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly object _lock = new object();

        public RequestAuthenticator(string apiKey, string apiSecret, string clientId)
        {
            _clientId = clientId;
            _apiKey = apiKey;
            _apiSecret = apiSecret;
        }

        public void Authenticate(RestRequest request)
        {
            lock (_lock)
            {
                string nonce = ApiUtilities.I.AscendingLongNext().ToString();
                request.AddParameter("key", _apiKey);
                request.AddParameter("nonce", nonce);
                request.AddParameter("signature", CreateSignature(nonce));
            }
        }

        private string CreateSignature(string nonce)
        {
            var msg = $"{nonce}{_clientId}{_apiKey}";
            return ByteArrayToString(SignHMACSHA256(_apiSecret, StringToByteArray(msg))).ToUpper();
        }

        private static byte[] SignHMACSHA256(string key, byte[] data)
        {
            var hashMaker = new HMACSHA256(Encoding.ASCII.GetBytes(key));
            return hashMaker.ComputeHash(data);
        }

        private static byte[] StringToByteArray(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        private static string ByteArrayToString(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}