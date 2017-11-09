using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IOrderBookProvider
    {
        Task<OrderBook> GetOrderBookAsync(OrderBookContext context);
    }
}
