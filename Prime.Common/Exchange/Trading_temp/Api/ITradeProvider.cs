using System.Threading.Tasks;

namespace Prime.Common
{
    public interface ITradeProvider : INetworkProviderPrivate
    {
        Task<PlacedTradeResponse> PlaceTradeAsync(PlaceTradeContext context);

        Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context);

        decimal MiniumumTradeVolume { get; }
    }
}