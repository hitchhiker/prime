using System.Linq;
using System.Security.Cryptography;
using System.Text;
using LiteDB;

namespace Prime.Utility
{
    public static class ObjectIdExtensionMethods
    {

        /// <summary>
        /// Will generate a reasonably unique consistant objectid from a string.
        /// </summary>
        /// <returns></returns>
        public static ObjectId GetObjectIdHashCode(this string k, bool autoLower = false, bool autoTrim = false)
        {
            if (string.IsNullOrWhiteSpace(k))
                return ObjectId.Empty;

            if (autoLower)
                k = k.ToLower();

            if (autoTrim)
                k = k.Trim();

            using (var algorithm = SHA256.Create())
            {
                // Create the at_hash using the access token returned by CreateAccessTokenAsync.
                var hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(k));
                var bytes = hash.ToArray().Take(12).ToArray();
                return new ObjectId(bytes);
            }
        }
    }
}