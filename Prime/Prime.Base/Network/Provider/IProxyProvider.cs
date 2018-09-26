namespace Prime.Core
{
    public interface IProxyProvider : INetworkProvider
    {
        string ProxyName { get; }
    }
}