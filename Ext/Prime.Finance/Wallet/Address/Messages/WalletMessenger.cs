using System;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;

namespace Prime.Finance.Wallet.Addresses
{
    public class WalletMessenger : IUserContextMessenger
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Instance;
        private readonly UserContext _userContext;
        private readonly string _token;
        private readonly WalletProvider _walletProvider;

        private WalletMessenger(UserContext userContext)
        {
            _userContext = userContext;
            _token = _userContext.Token;
            _walletProvider =  new WalletProvider(_userContext);
            _messenger.RegisterAsync<WalletAllRequestMessage>(this, _token, WalletAllRequestMessage);
            _messenger.RegisterAsync<WalletAddressRequestMessage>(this, _token, WalletAddressRequestMessage);
        }

        private async void WalletAddressRequestMessage(WalletAddressRequestMessage m)
        {
            var newAddresses = await _walletProvider.GenerateNewAddressAsync(m.Network, m.Asset).ConfigureAwait(false);

            if (newAddresses == null)
                return;
            
            foreach (var a in newAddresses)
                _messenger.SendAsync(new WalletAddressResponseMessage(a), _token);
        }

        public IUserContextMessenger GetInstance(UserContext context)
        {
            return new WalletMessenger(context);
        }

        private void WalletAllRequestMessage(WalletAllRequestMessage m)
        {
            _messenger.SendAsync(new WalletAllResponseMessage(_userContext.Finance().UserSettings.Addresses), _token);
        }

        public void Dispose()
        {
            _messenger.UnregisterAsync(this);
        }
    }
}