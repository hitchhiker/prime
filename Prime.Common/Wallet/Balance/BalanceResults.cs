using System.Linq;
using Prime.Utility;

namespace Prime.Common
{
    public class BalanceResults : UniqueList<BalanceResult>
    {
        public BalanceResults() { }

        public BalanceResults(IBalanceProvider provider)
        {
            ProviderSource = provider;
        }

        public readonly IBalanceProvider ProviderSource;

        public void Add(Asset asset, decimal available, decimal reserved)
        {
            var i = new BalanceResult(ProviderSource)
            {
                Available = new Money(available, asset),
                Reserved = new Money(reserved, asset),
                AvailableAndReserved = new Money(available + reserved, asset)
            };

            Add(i);
        }

        public void AddAvailable(Asset asset, decimal value)
        {
            if (value == 0)
                return;

            var i = new BalanceResult(ProviderSource) { Available = new Money(value, asset) };
            Add(i);
        }

        public void AddReserved(Asset asset, decimal value)
        {
            if (value == 0)
                return;

            var i = new BalanceResult(ProviderSource) {Reserved = new Money(value, asset)};
            Add(i);
        }

        public void AddAvailableReserved(Asset asset, decimal value)
        {
            if (value == 0)
                return;

            var i = new BalanceResult(ProviderSource) {AvailableAndReserved = new Money(value, asset)};
            Add(i);
        }
    }
}