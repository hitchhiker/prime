namespace Prime.Finance
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