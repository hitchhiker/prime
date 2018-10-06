using System;
using System.Threading.Tasks;
using Prime.Base;
using Prime.Base.DStore;
using Prime.Core;

namespace Prime.IPFS.Messaging
{
    public class NsHelper : CommonBase
    {
        private readonly IpfsMessenger _ipfsM;
        private readonly IpfsInstance _i;

        public NsHelper(IpfsMessenger ipfsM) : base(ipfsM)
        {
            _ipfsM = ipfsM;
            _i = _ipfsM.IpfsInstance;
        }

        public void Register()
        {
            M.RegisterAsync<PublishNsRequest>(_ipfsM, PublishNsRequest);
            M.RegisterAsync<GetNsResolveRequest>(_ipfsM, GetNsRequest);
        }

        private void PublishNsRequest(PublishNsRequest m)
        {
            if (m.Protocol != "ipfs")
                return;

            _i.StartAndDo(async delegate
            {
                var ci = await PublishNs(m);

                var response = new PublishNsResponse(m, "ipfs") { Success = ci!=null, ContentUri = new ContentUri(){Path = ci, Protocol = "ipfs"}};

                M.Send(response);
            });
        }

        private async Task<string> PublishNs(PublishNsRequest m)
        {
            var c = _i.Client;
            var attemptsLeft = 3;
            do
            {
                try
                {
                    var r = await c.Name.PublishAsync(m.LocalHash, m.Key, TimeSpan.FromDays(365 * 5));
                    return r.NamePath;
                }
                catch (Exception e)
                {
                    if (attemptsLeft-- <= 0)
                        throw;
                        
                    L.Log("NS Publish failed, trying again..");
                }
            } while (true);
        }

        private void GetNsRequest(GetNsResolveRequest m)
        {
            if (m.Protocol != "ipns")
                return;

            _i.StartAndDo(async delegate
            {
                var ci = await GetNs(m);

                var response = new GetNsResolveResponse(m, "ipfs") { Success = ci != null, ContentUri = new ContentUri() { Path = ci, Protocol = "ipfs" } };

                M.Send(response);
            });
        }

        private async Task<string> GetNs(GetNsResolveRequest m)
        {
            var c = _i.Client;

            var r = await c.Name.ResolveAsync(m.RemotePath);

            return r.Replace(@"/ipfs/","");
        }
    }
}