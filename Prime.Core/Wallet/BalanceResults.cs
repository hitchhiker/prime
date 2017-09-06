using System.Linq;
using Prime.Utility;

namespace Prime.Core
{
    public class BalanceResults : UniqueList<BalanceResult>
    {
        public BalanceResults() { }

        public BalanceResults(IWalletService provider)
        {
            ProviderSource = provider;
        }

        public readonly IWalletService ProviderSource;

        public void AddReserved(Asset asset, decimal value)
        {
            if (value == 0)
                return;

            var i = GetCurrency(asset);
            i.Reserved = new Money(value, asset);
        }

        public void AddBalance(Asset asset, decimal value)
        {
            if (value == 0)
                return;

            var i = GetCurrency(asset);
            i.Balance = new Money(value, asset);
        }

        public void AddAvailable(Asset asset, decimal value)
        {
            if (value == 0)
                return;

            var i = GetCurrency(asset);
            i.Available = new Money(value, asset);
        }

        private BalanceResult GetCurrency(Asset asset)
        {
            var i = this.FirstOrDefault(x => x.Asset == asset);
            if (i == null)
                Add(i = new BalanceResult(asset));
            return i;
        }

    }
}