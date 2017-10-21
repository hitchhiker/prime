namespace Prime.Common
{
    public static class AssetCustomColours
    {
        public static string CustomColor(Asset asset)
        {
            switch (asset.ShortCode)
            {
                case "BTC":
                    return "#ffb629";
                case "ETH":
                    return "#333";
                case "XRP":
                    return "#008fc8";
                default:
                    return null;
            }
        }
    }
}