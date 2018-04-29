using System;

namespace Prime.Core
{
    public interface IPriceSocketProvider : INetworkProvider
    {
        void SubscribePrice(Action<string, LivePriceResponse> action);

        void UnSubscribePrice();
    }
}