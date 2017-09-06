namespace Prime.Core
{
    public interface IOhlcProvider : INetworkProvider
    {
        OhclData GetOhlc(OhlcContext context);
    }
}