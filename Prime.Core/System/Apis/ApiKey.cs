using System;
using System.IO;
using System.Linq;
using Prime.Utility;
using LiteDB;

namespace Prime.Core
{
    public class ApiKey : IEquatable<ApiKey>
    {
        private ApiKey() { }

        public ApiKey(string name, string key, string secret, string extra = null)
        {
            Name = name;
            Secret = secret;
            Key = key;
            Extra = extra;
            Id = $"prime:api:{key ?? "key"}:{secret ?? "secret"}:{extra ?? "ex"}".GetObjectIdHashCode(true, true);
        }

        [BsonId]
        public ObjectId Id { get; private set; }

        [Bson]
        public string Name { get; private set; }

        /// <summary>
        /// Private Key or Secret Key
        /// </summary>
        [Bson]
        public string Secret { get; private set; }

        /// <summary>
        /// Public Key or Api Key
        /// </summary>
        [Bson]
        public string Key { get; private set; }

        /// <summary>
        /// Sometimes Apis require an extra field beyond pub/priv
        /// </summary>
        [Bson]
        public string Extra { get; private set; }

        public static ApiKey FromFile(FileInfo file)
        {
            if (!file.Exists)
                return null;

            var net = file.Name.Replace(".api.txt", "").ToLower().Trim();
            var p = File.ReadAllLines(file.FullName);
            if (p.Length < 2)
                return null;

            return new ApiKey(net + " imported", p[0], p[1], p.Length > 2 ? p[2] : null);
        }

        public bool Equals(ApiKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ApiKey) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}