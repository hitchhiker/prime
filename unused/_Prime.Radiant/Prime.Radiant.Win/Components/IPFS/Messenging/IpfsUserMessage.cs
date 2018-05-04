using System.Collections.Generic;
using Ipfs.Api;
using Newtonsoft.Json;
using Prime.Utility.Encrypt;

namespace Prime.Radiant.Components.IPFS.Messenging
{
    public class IpfsUserMessages
    {
        private readonly PrimeEncrypt _encrypt;

        public IpfsUserMessages() { }

        public IpfsUserMessages(PrimeEncrypt encrypt)
        {
            _encrypt = encrypt;
        }

        [JsonProperty]
        private Dictionary<string, string> Messages { get; set; } = new Dictionary<string, string>();

        public void AddOrUpdate(string itemKey, string value)
        {
            if (Messages.ContainsKey(itemKey))
                Messages.Remove(itemKey);

            Messages.Add(itemKey, _encrypt.Encrypt(value));
        }

        public void AddOrUpdate(string itemKey, FileSystemNode node)
        {
            AddOrUpdate(itemKey, node.Hash);
        }

        public void AddOrUpdate(string itemKey, object doc)
        {
            var raw = Newtonsoft.Json.JsonConvert.SerializeObject(doc);
            AddOrUpdate(itemKey, raw);
        }
    }
}