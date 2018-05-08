using Prime.Core;

namespace Prime.Finance
{
    public class FinanceUserContext
    {
        internal FinanceUserContext(UserContext context)
        {
            UserContext = context;
        }

        public readonly UserContext UserContext;

        private Asset _quoteAsset;

        public Asset QuoteAsset
        {
            get => _quoteAsset ?? (_quoteAsset = "USD".ToAssetRaw());
            set
            {
                var change = !Equals(_quoteAsset, value);
                _quoteAsset = value;
                if (change)
                    DefaultMessenger.I.Default.Send(new QuoteAssetChangedMessage(value));
            }
        }

        private WalletDatas _walletDatas;
        public WalletDatas WalletDatas => _walletDatas ?? (_walletDatas = new WalletDatas());

        public WalletData Data(IBalanceProvider provider) => WalletDatas.GetOrCreate(UserContext, provider);

        private readonly UserSettings _userdatas = new UserSettings();

        private UserSetting _userSettings;
        public UserSetting UserSettings => _userSettings ?? (_userSettings = _userdatas.GetOrCreate(UserContext));
    }
}