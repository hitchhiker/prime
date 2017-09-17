using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Radiant.Components
{
    public class PublishManager
    {
        public readonly IpFsDaemon IpfsDaemon;
        public readonly PublishManagerContext Context;
        public readonly ILogger L;
        private readonly PackageFiller _packageFiller;
        public string IpFsPeerId { get; private set; }

        public PublishManager(PublishManagerContext context)
        {
            Context = context;
            L = context.L;
            IpfsDaemon = new IpFsDaemon(Context.L, Context.Dispatcher);
            _packageFiller = new PackageFiller(this);
        }

        public void Start()
        {
            L.Info("Deployment starting");

            var bundlers = new List<PackageBundler>();

            if (Context.DoRadiant)
                bundlers.Add(new PackageBundler(this, "radiant", @"Prime.Radiant\Prime.Radiant.Wpf\bin\Debug"));

            if (Context.DoPrime)
                bundlers.Add(new PackageBundler(this, "prime", @"Prime.Ui\Wpf\bin\Debug"));

            foreach (var b in bundlers)
                _packageFiller.Fill(b);

            var api = new IpFsApi(IpfsDaemon);

            api.Connect();

            IpfsDaemon.WaitTillRunning(client =>
            {
                foreach (var b in bundlers)
                    Add(api, b);
                return true;
            });

            if (!string.IsNullOrWhiteSpace(Context.SshUri))
                SendSsh(bundlers);

            if (!string.IsNullOrWhiteSpace(Context.CloudFlareZoneId))
                foreach (var b in bundlers)
                    UpdateDns(b);

            L.Info("Deployment complete");
        }

        private void UpdateDns(PackageBundler packageBundler)
        {
            var cfapi = new CloudFlare(Context);
            var apiResult = cfapi.GetDnsRecords(Context.CloudFlareZoneId);
            if (apiResult == null)
            {
                L.Error("No results from the CloudFlare api");
                return;
            }

            var entry = $"{packageBundler.ProjectKey}-{Radiant.DnsVersion}.{Context.CloudFlareRootDomain}";

            var record = apiResult.result.FirstOrDefault(x =>
                string.Equals(x.name, entry, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.type, "TXT", StringComparison.OrdinalIgnoreCase));

            if (record == null)
            {
                L.Error("Cannot find TXT record for " + entry + "");
                return;
            }

            var updated = cfapi.UpdateDnsRecord(record.zone_id, record.id, record.type, record.name, packageBundler.Hash, 120);
            if (!updated.success)
            {
                L.Error("Unable to update TXT record for " + entry + "");
            }

            L.Info("Updated DNS record '" + entry + "' with hash: " + packageBundler.Hash);
        }

        private void SendSsh(List<PackageBundler> bundles)
        {
            var commands = bundles.Select(b => $"ipfs pin add {b.Hash}").ToList();

            commands.Insert(0, $"ipfs swarm connect /ip4/$clientip/tcp/4001/ipfs/{IpFsPeerId}");
            commands.Insert(0, "clientip=\"$(who am i --ips|awk '{print $5}')\"");

            new SSH.SshApi(Context).SendCommands(commands, c=> { Context.L.Info(c.Result); });
        }

        private void Add(IpFsApi api, PackageBundler packageBundler)
        {
            if (!packageBundler.Files.Any())
                return;

            try
            {
                var config = Task.Run(async () => await api.GetAddresses()).GetAwaiter().GetResult();
                IpFsPeerId = config["Identity"]["PeerID"].ToString();
            }catch { }

            Task.Run(async delegate
            {
                L.Info("Addding " + packageBundler.Packed.Count + " packages for " + packageBundler.ProjectKey + " to IPFS");

                var dir = packageBundler.PublishProjectDirectory;

                var hash = await api.Add(dir, true);

                L.Info("Pinned: " + packageBundler.ProjectKey + " " + hash);

                return packageBundler.Hash = hash;
            }).GetAwaiter().GetResult();
        }
    }
}
