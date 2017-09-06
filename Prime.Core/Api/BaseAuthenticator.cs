using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prime.Core
{
    public abstract class BaseAuthenticator
    {
        protected BaseAuthenticator(ApiKey apiKey)
        {
            ApiKey = apiKey;
        }

        public readonly ApiKey ApiKey;

        // ReSharper disable once InconsistentNaming
        public string HashHMACSHA512(string message, string secret)
        {
            return Convert.ToBase64String(HashHMACSHA512Raw(message, secret));
        }

        // ReSharper disable once InconsistentNaming
        public string HashHMACSHA256(string message, string secret)
        {
           return Convert.ToBase64String(HashHMACSHA256Raw(message, secret));
        }

        // ReSharper disable once InconsistentNaming
        public byte[] HashHMACSHA512Raw(string message, string secret)
        {
            using (var hmac = new HMACSHA512(FromUtf8(secret)))
            {
                var msg = FromUtf8(message);
                return hmac.ComputeHash(msg);
                // return BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
            }
        }

        // ReSharper disable once InconsistentNaming
        public byte[] HashHMACSHA256Raw(string message, string secret)
        {
            using (var hmac = new HMACSHA256(FromUtf8(secret)))
            {
                var bytes = FromUtf8(message);
                return hmac.ComputeHash(bytes);
            }
        }

        // ReSharper disable once InconsistentNaming
        public string HashHMACSHA256Hex(string message, string secret)
        {
            return ToHex(HashHMACSHA256Raw(message, secret));
        }        
        
        // ReSharper disable once InconsistentNaming
        public string HashHMACSHA512Hex(string message, string secret)
        {
            return ToHex(HashHMACSHA512Raw(message, secret));
        }

        public string ToHex(byte[] data)
        {
            return data.Aggregate(new StringBuilder(), (sb, b) => sb.AppendFormat("{0:x2}", b), sb => sb.ToString());
        }

        public byte[] FromUtf8(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public abstract void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken);

        public Task GetRequestModifier(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var t = new Task(() => RequestModify(request, cancellationToken));
            t.RunSynchronously();
            return t;
        }
    }
}