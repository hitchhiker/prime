using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IOrderBookProvider : IDescribesAssets
    {
        Task<OrderBook> GetOrderBookAsync(OrderBookContext context);
    }
}
