namespace Prime.Common
{
    public class AssetFoundMessage
    {
        public readonly Asset Asset;

        public AssetFoundMessage(Asset asset)
        {
            Asset = asset;
        }
    }
}