using System;

namespace Prime.Common
{
    public interface IPriceSocketProvider : INetworkProvider
    {
        void SubscribePrice(Action<string, LivePriceResponse> action);

        void UnSubscribePrice();
    }
}