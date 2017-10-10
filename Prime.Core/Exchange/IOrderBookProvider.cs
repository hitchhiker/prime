using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IOrderBookProvider
    {
        Task<OrderBook> GetOrderBook(OrderBookContext context);
    }
}
