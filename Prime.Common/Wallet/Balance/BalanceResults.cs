using System.Linq;
using Prime.Utility;

namespace Prime.Common
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

            var i = GetOrCreateBalanceResult(asset);
            i.Reserved = new Money(value, asset);
            Add(i);
        }

        public void AddBalance(Asset asset, decimal value)
        {
            if (value == 0)
                return;

            var i = GetOrCreateBalanceResult(asset);
            i.Balance = new Money(value, asset);
            Add(i);
        }

        public void AddAvailable(Asset asset, decimal value)
        {
            if (value == 0)
                return;

            var i = GetOrCreateBalanceResult(asset);
            i.Available = new Money(value, asset);
            Add(i);
        }

        private BalanceResult GetOrCreateBalanceResult(Asset asset)
        {
            var i = this.FirstOrDefault(x => Equals(x.Asset, asset));
            if (i == null)
                Add(i = new BalanceResult(asset));
            return i;
        }
    }
}