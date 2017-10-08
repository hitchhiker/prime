using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IOrderBookProvider
    {
        Task<OrderBook> GetOrderBookLive(OrderBookLiveContext context);
        Task<OrderBook> GetOrderBookHistory(OrderBookContext context);
    }
}
