namespace Prime.Common
{
    public interface IProxyProvider : INetworkProvider
    {
        string ProxyName { get; }
    }
}