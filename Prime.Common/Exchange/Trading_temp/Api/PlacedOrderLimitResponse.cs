namespace Prime.Common
{
    public class PlacedOrderLimitResponse
    {
        public PlacedOrderLimitResponse(string remoteTradeId)
        {
            RemoteTradeId = remoteTradeId;
        }

        public string RemoteTradeId { get; }
    }
}