﻿using System.Collections.Generic;

namespace Prime.Finance.Services.Services.CryptoCompare
{
    internal class CryptoCompareSchema
    {
        internal class CryptoCompareResponseBase
        {
            public string Response;
            public string Message;

            public bool IsError() => string.IsNullOrWhiteSpace(Response) || string.Equals(Response, "Error", System.StringComparison.OrdinalIgnoreCase);
        }

        internal class CoinEntry
        {
            public string Id;
            public string Url;
            public string ImageUrl;
            public string Name;
            public string CoinName;
            public string FullName;
            public string Algorithm;
            public string ProofType;
            public string FullyPremined;
            public string TotalCoinSupply;
            public string PreMinedValue;
            public string TotalCoinsFreeFloat;
            public string SortOrder;
        }

        internal class CoinSnapshotData
        {
            public string Algorithm;
            public string ProofType;
            public string BlockNumber;
            public string NetHashesPerSecond;
            public string TotalCoinsMined;
            public string BlockReward;
            public CoinSnapshotDataBlock AggregatedData;
            public List<CoinSnapshotDataBlock> Exchanges;
        }

        internal class CoinSnapshotDataBlock
        {
            public int TYPE;
            public string MARKET;
            public string FROMSYMBOL;
            public string TOSYMBOL;
            public int FLAGS;
            public string PRICE;
            public string LASTUPDATE;
            public string LASTVOLUME;
            public string LASTVOLUMETO;
            public string LASTTRADEID;
            public string VOLUME24HOUR;
            public string VOLUME24HOURTO;
            public string OPEN24HOUR;
            public string HIGH24HOUR;
            public string LOW24HOUR;
            public string LASTMARKET;
        }

        internal class CoinListResult : CryptoCompareResponseBase
        {
            public string BaseImageUrl;
            public string BaseLinkUrl;

            public Dictionary<string, CoinEntry> Data;
        }

        internal class HistoricListConversionType
        {
            public string type;
            public string conversionSymbol;
        }

        internal class HistoricEntry
        {
            public long time;
            public decimal high;
            public decimal low;
            public decimal open;
            public decimal close;
            public decimal volumefrom;
            public decimal volumeto;
        }

        internal class HistoricListResult : CryptoCompareResponseBase
        {
            public string Type;
            public bool Aggregated;
            public long TimeTo;
            public long TimeFrom;
            public bool FirstValueInArray;
            public HistoricListConversionType ConversionType;

            public List<HistoricEntry> Data;
        }

        internal class CoinSnapshotResult : CryptoCompareResponseBase
        {
            public string Type;
            public bool Aggregated;

            public CoinSnapshotData Data;
        }

        internal class AssetPairsAllExchanges : Dictionary<string, AssetPairsAllExchanges.AssetPairExchange>
        {
            public class AssetPairExchange : Dictionary<string, List<string>>
            {

            }
        }

        internal class TopExchangesResult : CryptoCompareResponseBase
        {
            public class TopExchangesItem
            {
                public string exchange;
                public string fromSymbol;
                public string toSymbol;
                public double volume24h;
                public double volume24hTo;
            }

            public List<TopExchangesItem> Data;
        }

        internal class PriceMultiResult : Dictionary<string, Dictionary<string, double>> { }
    }
}

