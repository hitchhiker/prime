using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IOrderBookProvider
    {
        Task<OrderBook> GetOrderBookLive(OrderBookContext context);
        Task<OrderBook> GetOrderBookHistory(OrderBookContext context);
    }
}
