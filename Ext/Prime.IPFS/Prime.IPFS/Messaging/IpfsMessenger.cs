using GalaSoft.MvvmLight.Messaging;
using Nito.AsyncEx;
using Prime.Core;

namespace Prime.IPFS.Messaging {
    public class IpfsMessenger
    {
        internal readonly PrimeContext S;
        internal readonly IpfsInstance IpfsInstance;
        public readonly IMessenger M;
        internal readonly ContentUriHelper ContentUriHelper;

        public IpfsMessenger(IpfsInstance ipfsInstance)
        {
            S = ipfsInstance.Context.PrimeContext;
            IpfsInstance = ipfsInstance;
            M = S.M;
            ContentUriHelper = new ContentUriHelper(this);
        }

        public void Start()
        {
            M.RegisterAsync<IpfsStartRequest>(this, IpfsStartRequest);
            M.RegisterAsync<IpfsStopRequest>(this, IpfsStopRequest);
            M.RegisterAsync<IpfsStatusRequest>(this, m=> SendIpfsStatus());
            M.RegisterAsync<IpfsVersionRequest>(this, m=> SendIpfsVersion());
            M.RegisterAsync<GetContentUriRequest>(this, ContentUriHelper.GetContentUriRequest);
        }

        private void IpfsStartRequest(IpfsStartRequest m)
        {
            IpfsInstance.StartDaemon();
        }

        private void IpfsStopRequest(IpfsStopRequest m)
        {
            IpfsInstance.StopDaemon();
        }

        private void SendIpfsVersion()
        {
            if (IpfsInstance.Daemon.CurrentState == DaemonState.Running)
            {
                AsyncContext.Run(async () =>
                {
                    var v = await IpfsInstance.Client.VersionAsync();
                    M.SendAsync(new IpfsVersionResponse() { Version = v.Get("Version")/*, Item = v*/ });
                });
            }
        }

        public void SendIpfsStatus()
        {
            M.SendAsync(new IpfsStatusMessage(IpfsInstance.Daemon.State().GetServiceStatus()));
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
