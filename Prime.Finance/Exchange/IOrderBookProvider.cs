using System.Threading.Tasks;

namespace Prime.Finance
{
    public interface IOrderBookProvider : IDescribesAssets
    {
        Task<OrderBook> GetOrderBookAsync(OrderBookContext context);
    }
}
