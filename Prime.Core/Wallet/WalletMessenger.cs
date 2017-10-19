using System;
using System.Linq;
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
            _messenger.RegisterAsync<WalletAddressRequestMessage>(this, _token, WalletAddressRequestMessage);
        }

        private async void WalletAddressRequestMessage(WalletAddressRequestMessage m)
        {
            var newAddresses = await _userContext.WalletProvider.GenerateNewAddress(m.Network, m.Asset);

            if (newAddresses == null)
                return;
            
            foreach (var a in newAddresses)
                _messenger.SendAsync(new WalletAddressResponseMessage(a), _userContext.Token);
        }

        public IUserContextMessenger GetInstance(UserContext context)
        {
            return new WalletMessenger(context);
        }

        private void WalletAllRequestMessage(WalletAllRequestMessage m)
        {
            _messenger.SendAsync(new WalletAllResponseMessage(_userContext.UserSettings.Addresses), _token);
        }

        public void Dispose()
        {
            _messenger.UnregisterAsync(this);
        }
    }
}