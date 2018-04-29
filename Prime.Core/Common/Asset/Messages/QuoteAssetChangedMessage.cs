namespace Prime.Core
{
    public class QuoteAssetChangedMessage
    {
        public readonly Asset NewAsset;

        public QuoteAssetChangedMessage(Asset newAsset)
        {
            NewAsset = newAsset;
        }
    }
}