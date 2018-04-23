using System.Threading.Tasks;
using Ipfs.Api;
using Newtonsoft.Json;
using Prime.Utility;
using Prime.Utility.Encrypt;

namespace Prime.Radiant.Components.IPFS.Messenging
{
    public class IpfsMessenger
    {
        private IpfsMessenger() { }

        public IpfsMessenger(Radiant radiant, string userKey, PrimeEncrypt encrypt)
        {
            _radiant = radiant;
            _encrypt = encrypt;
            UserKey = userKey;
            UserMessages = new IpfsUserMessages(_encrypt);
        }

        private string _pubKey;
        private string _privKey;
        private readonly Radiant _radiant;
        private readonly PrimeEncrypt _encrypt;

        [JsonProperty]
        public string UserKey { get; private set; }

        [JsonProperty]
        public IpfsUserMessages UserMessages { get; private set; }

        public async Task<string> Publish()
        {
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            var na = _radiant.IpfsDaemon.Client;

            FileSystemNode node;

            using (var mstream = message.ToStream())
                node = await na.FileSystem.AddAsync(mstream);
            
            //await na.Pin.AddAsync(node.Hash);

            var result = await na.DoCommandAsync("name//publish", new System.Threading.CancellationToken(), node.Hash);

            return result;
        }

        public async Task<IpfsMessenger> Retrieve(string hash)
        {
            var c = _radiant.IpfsDaemon.Client;
            var result = await c.DoCommandAsync("name//resolve", new System.Threading.CancellationToken(), hash);
            var pResult = Newtonsoft.Json.JsonConvert.DeserializeObject<IpfsPathResponse>(result);
            var txt = await c.FileSystem.ReadAllTextAsync(pResult.Path);
            var msg = Newtonsoft.Json.JsonConvert.DeserializeObject<IpfsMessenger>(txt);
            return msg;
        }

        public void GetKeys()
        {
            if (_pubKey != null)
                return;

            var keys = _radiant.IpFsApi.GetIpfsKeys();
            _pubKey = keys.pubKey;
            _privKey = keys.privKey;
        }
    }
}
