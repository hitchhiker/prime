namespace Prime.Core.Wallet
{
    public class BaseAssetChangedMessage
    {
        public readonly Asset NewAsset;

        public BaseAssetChangedMessage(Asset newAsset)
        {
            NewAsset = newAsset;
        }
    }
}