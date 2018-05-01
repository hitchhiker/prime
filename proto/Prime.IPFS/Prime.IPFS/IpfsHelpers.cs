using System;
using System.IO;
using System.Threading.Tasks;
using Ipfs;
using Ipfs.Api;
using Newtonsoft.Json.Linq;
using Nito.AsyncEx;
using Prime.Core;

namespace Prime.IPFS
{
    public class IpfsHelpers
    {
        private readonly IpfsDaemonBase _daemon;
        private readonly ILogger L;
        private readonly IpfsClient _client;

        public IpfsHelpers(IpfsDaemonBase daemon)
        {
            _daemon = daemon;
            _client = new IpfsClient();
        }

        public void Connect()
        {
            _daemon.Start();
        }

        public (string privKey, string pubKey) GetIpfsKeys()
        {
            return _daemon.WaitTillRunning(() =>
            {
                try
                {
                    var api = _client.Config.GetAsync().Result;

                    var pub = api["Identity"]["PeerID"].ToString();

                    var di = new DirectoryInfo(api["Datastore"]["Path"].ToString()).Parent;

                    var conf = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(
                        File.ReadAllText(Path.Combine(di.FullName, "config")));

                    var priv = conf["Identity"]["PrivKey"].ToString();

                    return (priv, pub);
                }
                catch (Exception e)
                {
                    L.Error(e, "Problems retrieving IPFS keys");
                }
                return (null, null);
            });
        }

        private void GetIpfsFile(string hash, long size, string name)
        {
            L.Info("Requesting data stream from IPFS (" + size + " bytes)");

            using (var f = AsyncContext.Run(() => _client.FileSystem.ReadFileAsync(hash)))
            {
                //deployer.GetFromStream(f, size, name);
            }

            PinIpfsFile(hash);
        }

        private void PinIpfsFile(string hash, bool recursive = true)
        {
            var pinned = AsyncContext.Run(() => _client.Pin.AddAsync(hash, recursive));

            if (pinned != null)
                return;

            L.Error("Node could not be pinned.");
        }

        public async Task<JObject> GetAddresses()
        {
            return await _client.Config.GetAsync();
        }

        public async Task<bool> SwarmConnect(string endpoint)
        {
            try
            {
                L.Log("[Optimisation] Connecting to distant peer swarm: " + endpoint);
                await _client.Swarm.ConnectAsync(new MultiAddress(endpoint));
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<string> Add(DirectoryInfo directory, bool pin = false)
        {
            var deployNode = await _client.FileSystem.AddDirectoryAsync(directory.FullName, true);

            if (!deployNode.IsDirectory)
            {
                L.Error("HASH did not refer to a directory node.");
                return null;
            }
            /*
            PinnedObject[] pinned = null;
            if (pin)
                pinned = await _client.Pin.AddAsync(deployNode.Hash, true);

            if (pin && pinned == null)
            {
                L.Error("Node could not be pinned.");
                return null;
            }

            return deployNode.Hash;*/

            return "";
        }
    }
}