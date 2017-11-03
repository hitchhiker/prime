using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Utility;

namespace Prime.Tests
{
    [TestClass]
    public class AssetDiscovery
    {
        [TestMethod]
        public void TestCompareAggregatorDataWithAssetPairData()
        {
            var pub = PublicContext.I.PubData;
            var d = new Dictionary<Network, AssetPairs>();
            foreach (var prov in Networks.I.AssetPairsProviders)
            {
                var r = ApiCoordinator.GetAssetPairs(prov);
                if (r.IsFailed)
                    Assert.Fail(prov.Title + " could not perform " + nameof(ApiCoordinator.GetAssetPairs));

                d.Add(prov.Network, r.Response);
            }

            var pairs = d.SelectMany(x => x.Value).ToUniqueList();

            foreach (var pair in pairs)
            {
                var apd = pub.GetAggAssetPairData(pair);
                var aggNets = apd.Exchanges.Select(x => x.Network).ToUniqueList();
                var apNets = d.Where(x => x.Value.Contains(pair)).Select(x => x.Key).ToUniqueList();

                foreach (var n in apNets)
                {
                    if (aggNets.Contains(n))
                        continue;
                    Assert.Fail($"Aggregated data for {pair} does not contain network: {n.Name}");
                }
            }
        }
    }
}
