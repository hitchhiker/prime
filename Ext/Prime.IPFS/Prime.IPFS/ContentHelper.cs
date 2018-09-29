using System.IO;
using System.Threading.Tasks;
using Ipfs.CoreApi;
using Nito.AsyncEx;
using Prime.Base;
using Prime.Base.DStore;
using Prime.Core;

namespace Prime.IPFS.Messaging
{
    public class ContentHelper : CommonBase
    {
        private readonly IpfsMessenger _ipfsM;
        private readonly IpfsInstance _i;

        public ContentHelper(IpfsMessenger ipfsM) : base(ipfsM)
        {
            _ipfsM = ipfsM;
            _i = _ipfsM.IpfsInstance;
        }

        public void Register()
        {
            M.RegisterAsync<GetContentUriRequest>(_ipfsM, GetContentUriRequest);
            M.RegisterAsync<GetContentRequest>(_ipfsM, GetContentRequest);
            M.RegisterAsync<PinContentRequest>(_ipfsM, PinContentRequest);
        }

        private void PinContentRequest(PinContentRequest m)
        {
            if (m.Protocol != "ipfs")
                return;

            _i.StartAndDo(async delegate
            {
                var ci = await PinContent(m);

                var response = new PinContentResponse(m, "ipfs") { Success = ci };

                M.Send(response);
            });
        }

        private async Task<bool> PinContent(PinContentRequest m)
        {
            var c = _i.Client;

            await c.Pin.AddAsync(m.LocalPath, true);

            return true;
        }

        private void GetContentRequest(GetContentRequest m)
        {
            if (m.Protocol != "ipfs")
                return;

            if (m.IsDirectory)
            {
                Directory.CreateDirectory(m.LocalPath);
                M.Send(new GetContentResponse(m, "ipfs") { Success = GetDirectoryRequest(m) });
                return;
            }

            _i.StartAndDo(async delegate
            {
                var ci = await GetContent(m);

                var response = new GetContentResponse(m, "ipfs") {Success = ci};

                M.Send(response);
            });
        }

        private bool GetDirectoryRequest(GetContentRequest m)
        {
            var rootNode = AsyncContext.Run(() => _i.Client.FileSystem.ListFileAsync(m.RemotePath));

            if (rootNode.IsDirectory)
            {
                foreach (var node in rootNode.Links)
                    GetFile(m, node.Id, node.Size, node.Name);
            }
            else
                GetFile(m, rootNode.Id, rootNode.Size, rootNode.Id);

            return true;
        }

        private void GetFile(GetContentRequest m, string hash, long size, string name)
        {
            L.Info("Requesting data stream from IPFS (" + size + " bytes)");

            AsyncContext.Run(async () =>
            {
                var path = Path.Combine(m.LocalPath, name);
                if (File.Exists(path))
                    File.Delete(path);

                using (var file = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    using (var sourceStream = await _i.Client.FileSystem.ReadFileAsync(hash))
                    {
                        await sourceStream.CopyToAsync(file);
                    }
                }
            });
        }

        private async Task<bool> GetContent(GetContentRequest m)
        {
            var c = _i.Client;

            using (var file = new FileStream(m.LocalPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                using (var sourceStream = await c.FileSystem.ReadFileAsync(m.RemotePath))
                {
                    await sourceStream.CopyToAsync(file);
                }
            }

            return true;
        }

        private void GetContentUriRequest(GetContentUriRequest m)
        {
            if (m.Protocol != "ipfs")
                return;

            _i.StartAndDo(async delegate
            {
                var ci = await GetContentUri(m);

                var response = new GetContentUriResponse(m, "ipfs");

                if (ci.Path != null)
                {
                    response.Success = true;
                    response.ContentUri = ci;
                }

                M.Send(response);
            });
        }

        private async Task<ContentUri> GetContentUri(GetContentUriRequest m)
        {
            var c = _i.Client;
            var type = m.LocalPath.DetectFileSystemPath();

            if (type==0)
                return new ContentUri();

            var node = type == 2
                ? await c.FileSystem.AddDirectoryAsync(m.LocalPath)
                : await c.FileSystem.AddFileAsync(m.LocalPath);

            return node==null ? new ContentUri() : new ContentUri() {Protocol = "ipfs", Path = node.Id.Hash.ToBase58()};
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