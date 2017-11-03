using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Utility;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public class FrankTests
        {
            public FrankTests()
            {
                /*
                var pub = PublicContext.I.PubData;
                var d = new Dictionary<Network, AssetPairs>();
                foreach (var prov in Networks.I.AssetPairsProviders)
                {
                    var r = ApiCoordinator.GetAssetPairs(prov);
                    if (r.IsFailed)
                    {
                        Console.WriteLine(prov.Title + " could not perform " + nameof(ApiCoordinator.GetAssetPairs));
                        return;
                    }

                    d.Add(prov.Network, r.Response);
                }

                var pairs = d.SelectMany(x => x.Value).ToUniqueList();

                foreach (var pair in pairs)
                {
                    var apd = pub.GetAggAssetPairData(pair);
                    if (apd.IsMissing)
                    {
                        Console.WriteLine($"No AGG data for {pair}");
                        continue;
                    }

                    var aggNets = apd.Exchanges.Select(x => x.Network).ToUniqueList();
                    var apNets = d.Where(x => x.Value.Contains(pair)).Select(x => x.Key).ToUniqueList();

                    foreach (var n in apNets)
                    {
                        if (aggNets.Contains(n))
                            continue;
                        Console.WriteLine($"{pair} not found via network: {n.Name}");
                    }
                }*/
            }
        }
    }
}