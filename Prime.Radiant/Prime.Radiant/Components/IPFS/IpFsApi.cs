using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ipfs.Api;
using Prime.Utility;
using Ipfs;
using Newtonsoft.Json.Linq;
using Nito.AsyncEx;
using Prime.Radiant.Components;

namespace Prime.Radiant
{
    public class IpFsApi
    {
        private readonly IpFsDaemon _daemon;
        private readonly Logger L;
        private readonly IpfsClient _client;

        public IpFsApi(IpFsDaemon daemon)
        {
            _daemon = daemon;
            _client = daemon.Client;
            L = daemon.L;
        }

        public IpFsApi(DeploymentManager manager) : this(manager.Daemon) { }

        public void Connect()
        {
            L.Info("Connecting to the decentralised internet [IPFS]");
            _daemon.Start();
        }

        public bool GetDeployment(DeploymentProvider deployer)
        {
            //var newTrustedPeer = new MultiAddress("/ip4/178.79.181.244/tcp/4001/ipfs/QmSxrFXq85asEujMX7HpRRZP98cPsYDCnpTtqBuCh5DBKk");
            //client.TrustedPeers.Add(newTrustedPeer);

            var rootNode = AsyncContext.Run(() => _client.FileSystem.ListFileAsync(deployer.Hash));

            if (rootNode.IsDirectory)
            {
                foreach (var node in rootNode.Links)
                    GetIpfsFile(deployer, node.Hash, node.Size, node.Name);
            }
            else
                GetIpfsFile(deployer, rootNode.Hash, rootNode.Size, rootNode.Name);

            deployer.Decompress();

            deployer.SetComplete();

            return true;
        }

        public (string privKey, string pubKey) GetIpfsKeys()
        {
            return _daemon.WaitTillRunning(client =>
            {
                try
                {
                    var api = client.Config.GetAsync().Result;

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

        private void GetIpfsFile(DeploymentProvider deployer, string hash, long size, string name)
        {
            L.Info("Requesting data stream from IPFS (" + size + " bytes)");

            using (var f = AsyncContext.Run(() => _client.FileSystem.ReadFileAsync(hash)))
            {
                deployer.GetFromStream(f, size, name);
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

        public async Task<string> Add(DirectoryInfo directory, bool pin = false)
        {
            var deployNode = await _client.FileSystem.AddDirectoryAsync(directory.FullName, true);

            if (!deployNode.IsDirectory)
            {
                L.Error("HASH did not refer to a directory node.");
                return null;
            }

            PinnedObject[] pinned = null;
            if (pin)
                pinned = await _client.Pin.AddAsync(deployNode.Hash, true);

            if (pin && pinned == null)
            {
                L.Error("Node could not be pinned.");
                return null;
            }

            return deployNode.Hash;
        }
    }
}