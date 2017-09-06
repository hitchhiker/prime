using System.Collections.Generic;

namespace KrakenApi
{
    public class GetAssetPairsResponse : ResponseBase
    {
        public Dictionary<string, AssetPair> Result;
    }
}