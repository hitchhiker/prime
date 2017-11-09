using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class WalletData : ModelBase, IKeepFresh, IOnNewInstance
    {
        public static object Lock = new Object();

        [BsonField("wallets")]
        private UniqueList<WalletAddress> _ws { get; set; } = new UniqueList<WalletAddress>();

        public IReadOnlyList<WalletAddress> Addresses => _ws;

        public void Add(WalletAddress address)
        {
            _ws.Add(address, true);
        }

        public void AddRange(IEnumerable<WalletAddress> addresses)
        {
            _ws.AddRange(addresses, true);
        }

        public WalletAddress GetLatest(Asset asset)
        {
            return _ws.Where(x=> !x.IsExpired()).OrderByDescending(x => x.UtcCreated).FirstOrDefault(x => x.Asset == asset);
        }

        public IReadOnlyList<WalletAddress> Get(Asset asset)
        {
            return _ws.Where(x => !x.IsExpired()).OrderByDescending(x => x.UtcCreated).ToList();
        }

        public void KeepFresh(UserContext context, IBalanceProvider provider, Asset asset, bool forceStale = false)
        {
            var stale = forceStale || _ws.Any(x => x.Asset == asset && (!x.IsFresh() || x.IsExpired()));
            if (!stale)
                return;

            lock (Lock)
            {
                var r = ApiCoordinator.FetchAllDepositAddresses(provider, new WalletAddressAssetContext(asset, context));
                if (r.IsNull)
                    return;

                AddRange(r.Response);
                this.Save(context);
            }
        }

        public void AfterCreation(IDataContext context, IUniqueIdentifier<ObjectId> parentObject) {}
    }
}