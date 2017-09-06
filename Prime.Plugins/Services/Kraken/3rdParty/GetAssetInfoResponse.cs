using System.Collections.Generic;

namespace KrakenApi
{
    public class GetAssetInfoResponse : ResponseBase
    {
        public Dictionary<string, AssetInfo> Result;
    }
}