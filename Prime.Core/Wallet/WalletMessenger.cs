using System;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Core
{
    public class WalletMessenger : IUserContextMessenger
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        private readonly UserContext _userContext;
        private readonly string _token;

        private WalletMessenger(UserContext userContext)
        {
            _userContext = userContext;
            _token = _userContext.Token;
            _messenger.RegisterAsync<WalletAllRequestMessage>(this, _token, WalletAllRequestMessage);
        }

        public IUserContextMessenger GetInstance(UserContext context)
        {
            return new WalletMessenger(context);
        }

        private void WalletAllRequestMessage(WalletAllRequestMessage m)
        {
            _messenger.Send(new WalletAllResponseMessage(null), _token);
        }

        public void Dispose()
        {
            _messenger.UnregisterAsync(this);
        }
    }
}