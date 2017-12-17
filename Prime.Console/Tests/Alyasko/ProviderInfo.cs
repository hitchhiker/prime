using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Common;

namespace Prime.Console.Tests.Alyasko
{
    public class ProviderInfo : ITestBase
    {
        private List<Type> _supportedInterfaces = new List<Type>()
        {
            typeof(IPublicPricingProvider),
            typeof(IDepositProvider),
            typeof(IOrderBookProvider),
            typeof(IBalanceProvider),
            typeof(IOhlcProvider),
            typeof(IAssetPairsProvider),
            typeof(IPublicVolumeProvider),
            typeof(IWithdrawalPlacementProviderExtended),
            typeof(IWithdrawalHistoryProvider),
            typeof(IWithdrawalCancelationProvider),
            typeof(IWithdrawalConfirmationProvider),
        };

        public ProviderInfo(INetworkProvider baseProvider)
        {
            NetworkProvider = baseProvider;
        }

        public void PrintReadableInfo()
        {
            var info = String.Empty;
            foreach (var type in _supportedInterfaces)
            {
                var sType = NetworkProvider.GetType();
                if (sType.GetInterfaces().Contains(type))
                    info += " - " + type.Name + "\n";
            }

            System.Console.WriteLine($"'{NetworkProvider.Network.Name}':\n{info}");
        }

        public INetworkProvider NetworkProvider { get; set; }

        public bool ProviderSupports<TInterface>()
        {
            return NetworkProvider is TInterface;
        }

        public void Go()
        {
            PrintReadableInfo();
        }
    }
}
