using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prime.Common
{
    public abstract class BaseAuthenticator
    {
        protected BaseAuthenticator(ApiKey apiKey)
        {
            ApiKey = apiKey;
        }

        public readonly ApiKey ApiKey;

        #region Nonce

        private static readonly long ArbTickEpoch = new DateTime(1990, 1, 1).Ticks;

        public static long GetLongNonce()
        {
            return DateTime.UtcNow.Ticks;
        }

        public static long GetUnixEpochNonce()
        {
            return DateTime.UtcNow.Ticks - ArbTickEpoch;
        }

        protected virtual long GetNonce()
        {
            return GetLongNonce();
        }

        #endregion

        #region Hash SHA256

        // ReSharper disable once InconsistentNaming
        public string HashSHA256(string message)
        {
            using (var sha256 = SHA256.Create())
            {
                return ToHex(sha256.ComputeHash(FromUtf8(message)));
            }
        }

        // ReSharper disable once InconsistentNaming
        public byte[] HashSHA256Raw(string message)
        {
            using (SHA256 hash = SHA256.Create())
            {
                return hash.ComputeHash(FromUtf8(message));
            }
        }

        #endregion

        #region Hash SHA384

        // ReSharper disable once InconsistentNaming
        public string HashSHA384(string message)
        {
            using (var sha384 = SHA384.Create())
            {
                return ToHex(sha384.ComputeHash(FromUtf8(message)));
            }
        }

        #endregion

        #region Hash HMACSHA256

        // ReSharper disable once InconsistentNaming
        public string HashHMACSHA256(string message, string secret)
        {
            return ToBase64(HashHMACSHA256Raw(message, secret));
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
        public byte[] HashHMACSHA256Raw(byte[] message, byte[] secret)
        {
            using (var hmac = new HMACSHA256(secret))
            {
                return hmac.ComputeHash(message);
            }
        }

        // ReSharper disable once InconsistentNaming
        public string HashHMACSHA256Hex(string message, string secret)
        {
            return ToHex(HashHMACSHA256Raw(message, secret));
        }

        #endregion

        #region Hash HMACSHA384

        // ReSharper disable once InconsistentNaming
        public string HashHMACSHA384(string message, string secret)
        {
            return ToBase64(HashHMACSHA384Raw(message, secret));
        }

        // ReSharper disable once InconsistentNaming
        public string HashHMACSHA384Hex(string message, string secret)
        {
            return ToHex(HashHMACSHA384Raw(message, secret));
        }

        // ReSharper disable once InconsistentNaming
        public byte[] HashHMACSHA384Raw(string message, string secret)
        {
            using (var hmac = new HMACSHA384(FromUtf8(secret)))
            {
                var msg = FromUtf8(message);
                return hmac.ComputeHash(msg);
            }
        }

        #endregion

        #region Hash HMACSHA512

        // ReSharper disable once InconsistentNaming
        public string HashHMACSHA512(string message, string secret)
        {
            return ToBase64(HashHMACSHA512Raw(message, secret));
        }

        // ReSharper disable once InconsistentNaming
        public string HashHMACSHA512(byte[] message, byte[] secret)
        {
            using (var hmacsha512 = new HMACSHA512(secret))
                return ToBase64(hmacsha512.ComputeHash(message));
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
        public string HashHMACSHA512Hex(string message, string secret)
        {
            return ToHex(HashHMACSHA512Raw(message, secret));
        }

        #endregion

        #region Hash MD5

        // ReSharper disable once InconsistentNaming
        public byte[] HashMD5Raw(string message)
        {
            using (MD5 md5 = MD5.Create())
            {
                return md5.ComputeHash(FromUtf8(message));
            }
        }

        // ReSharper disable once InconsistentNaming
        public string HashMD5Hex(string message)
        {
            return ToHex(HashMD5Raw(message));
        }

        // ReSharper disable once InconsistentNaming
        public string HashMD5(string message)
        {
            return ToBase64(HashMD5Raw(message));
        }

        #endregion

        #region Convert To

        public string ToHex(byte[] data)
        {
            return data.Aggregate(new StringBuilder(), (sb, b) => sb.AppendFormat("{0:x2}", b), sb => sb.ToString());
        }

        public string ToBase64(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        public string ToBase64(string data)
        {
            return ToBase64(FromUtf8(data));
        }

        public string ToUtf8(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        #endregion

        #region Convert From

        public byte[] FromUtf8(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }
        public byte[] FromBase64(string data)
        {
            return Convert.FromBase64String(data);
        }

        #endregion

        public abstract void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken);

        public Task GetRequestModifierAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Run(() => RequestModify(request, cancellationToken), cancellationToken);
        }
    }
}