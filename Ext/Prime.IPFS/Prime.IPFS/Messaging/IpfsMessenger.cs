using GalaSoft.MvvmLight.Messaging;
using Nito.AsyncEx;
using Prime.Core;

namespace Prime.IPFS.Messaging {

    public class IpfsMessenger
    {
        private readonly ServerContext _context;
        private readonly IpfsInstance _ipfs;
        public readonly IMessenger M;

        public IpfsMessenger(IpfsInstance ipfs)
        {
            _context = ipfs.Context.ServerContext;
            _ipfs = ipfs;
            M = _context.M;
        }

        public void Start()
        {
            M.RegisterAsync<IpfsStartRequest>(this, IpfsStartRequest);
            M.RegisterAsync<IpfsStopRequest>(this, IpfsStopRequest);
            M.RegisterAsync<IpfsStatusRequest>(this, m=> SendIpfsStatus());
            M.RegisterAsync<IpfsVersionRequest>(this, m=> SendIpfsVersion());
        }

        private void IpfsStartRequest(IpfsStartRequest m)
        {
            _ipfs.Start();
        }

        private void IpfsStopRequest(IpfsStopRequest m)
        {
            _ipfs.Stop();
        }

        private void SendIpfsVersion()
        {
            if (_ipfs.Daemon.CurrentState == DaemonState.Running)
            {
                AsyncContext.Run(async () =>
                {
                    var v = await _ipfs.Client.VersionAsync();
                    M.SendAsync(new IpfsVersionResponse() { Version = v.Get("Version")/*, Item = v*/ });
                });
            }
        }

        public void SendIpfsStatus()
        {
            M.SendAsync(new IpfsStatusMessage(_ipfs.Daemon.State().GetServiceStatus()));
        }

        public void Stop()
        {
            M.UnregisterAsync(this);
        }
    
        /*
        private IpfsMessenger() { }

        public IpfsMessenger(Prime.IPFS radiant, string userKey, PrimeEncrypt encrypt)
        {
            _radiant = radiant;
            _encrypt = encrypt;
            UserKey = userKey;
            UserMessages = new IpfsUserMessages(_encrypt);
        }

        private string _pubKey;
        private string _privKey;
        private readonly Prime.IPFS _radiant;
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
        }*/
    }
}
