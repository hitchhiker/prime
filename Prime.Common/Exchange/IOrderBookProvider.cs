using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IOrderBookProvider
    {
        Task<OrderBook> GetOrderBook(OrderBookContext context);
    }
}
