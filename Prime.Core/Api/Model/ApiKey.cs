using System;
using System.IO;
using System.Linq;
using Prime.Utility;
using LiteDB;

namespace Prime.Core
{
    public class ApiKey : ModelBase, IEquatable<ApiKey>
    {
        private ApiKey() { }

        public ApiKey(Network network, string name, string key, string secret, string extra = null)
        {
            Network = network;
            Name = name;
            Secret = secret;
            Key = key;
            Extra = extra;
            Id = $"prime:net:{Network.NameLowered}:api:{key ?? "key"}:{secret ?? "secret"}:{extra ?? "ex"}".GetObjectIdHashCode(true, true);
        }
        
        [Bson]
        public Network Network { get; private set; }

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

        public static ApiKey FromFile(Network network, FileInfo file)
        {
            if (!file.Exists)
                return null;

            var p = File.ReadAllLines(file.FullName);
            if (p.Length < 2)
                return null;

            return new ApiKey(network, network.Name + " imported", p[0], p[1], p.Length > 2 ? p[2] : null);
        }

        public bool Equals(ApiKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Network, other.Network) && string.Equals(Key, other.Key);
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
            unchecked
            {
                return ((Network != null ? Network.GetHashCode() : 0) * 397) ^ (Key != null ? Key.GetHashCode() : 0);
            }
        }
    }
}