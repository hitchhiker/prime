using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.CryptoCompare
{
    internal class CryptoCompareSchema
    {
        internal class CryptoCompareResponseBase
        {
            public string Response { get; set; }
            public string Message { get; set; }

            public bool IsError() => string.IsNullOrWhiteSpace(Response) || string.Equals(Response, "Error", System.StringComparison.OrdinalIgnoreCase);
        }

        internal class CoinEntry
        {
            public string Id { get; set; }
            public string Url { get; set; }
            public string ImageUrl { get; set; }
            public string Name { get; set; }
            public string CoinName { get; set; }
            public string FullName { get; set; }
            public string Algorithm { get; set; }
            public string ProofType { get; set; }
            public string FullyPremined { get; set; }
            public string TotalCoinSupply { get; set; }
            public string PreMinedValue { get; set; }
            public string TotalCoinsFreeFloat { get; set; }
            public string SortOrder { get; set; }
        }

        internal class CoinSnapshotData
        {
            public string Algorithm { get; set; }
            public string ProofType { get; set; }
            public string BlockNumber { get; set; }
            public string NetHashesPerSecond { get; set; }
            public string TotalCoinsMined { get; set; }
            public string BlockReward { get; set; }
            public CoinSnapshotDataBlock AggregatedData { get; set; }
            public List<CoinSnapshotDataBlock> Exchanges { get; set; }
        }

        internal class CoinSnapshotDataBlock
        {
            public string TYPE;
            public string MARKET;
            public string FROMSYMBOL;
            public string TOSYMBOL;
            public string FLAGS;
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
            public string BaseImageUrl { get; set; }
            public string BaseLinkUrl { get; set; }

            public Dictionary<string, CoinEntry> Data { get; set; }
        }

        internal class HistoricListConversionType
        {
            public string type { get; set; }
            public string conversionSymbol { get; set; }
        }

        internal class HistoricEntry
        {
            public long time { get; set; }
            public double high { get; set; }
            public double low { get; set; }
            public double open { get; set; }
            public double close { get; set; }
            public double volumefrom { get; set; }
            public double volumeto { get; set; }
        }

        internal class HistoricListResult : CryptoCompareResponseBase
        {
            public string Type { get; set; }
            public bool Aggregated { get; set; }
            public long TimeTo { get; set; }
            public long TimeFrom { get; set; }
            public bool FirstValueInArray { get; set; }
            public HistoricListConversionType ConversionType { get; set; }

            public List<HistoricEntry> Data { get; set; }
        }

        internal class CoinSnapshotResult : CryptoCompareResponseBase
        {
            public string Type { get; set; }
            public bool Aggregated { get; set; }

            public CoinSnapshotData Data { get; set; }
        }

    }
}

