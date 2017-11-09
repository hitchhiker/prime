using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IOrderBookProvider : IDescribesAssets
    {
        Task<OrderBook> GetOrderBookAsync(OrderBookContext context);
    }
}
