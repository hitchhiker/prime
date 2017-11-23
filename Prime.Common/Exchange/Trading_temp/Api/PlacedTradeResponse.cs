namespace Prime.Common
{
    public class PlacedTradeResponse
    {
        public PlacedTradeResponse(string remoteTradeId)
        {
            RemoteTradeId = remoteTradeId;
        }

        public string RemoteTradeId { get; }
    }
}