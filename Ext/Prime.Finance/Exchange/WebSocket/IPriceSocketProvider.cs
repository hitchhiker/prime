using System;
using Prime.Core;

namespace Prime.Finance
{
    public interface IPriceSocketProvider : INetworkProvider
    {
        void SubscribePrice(Action<string, LivePriceResponse> action);

        void UnSubscribePrice();
    }
}