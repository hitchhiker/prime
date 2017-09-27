using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Kraken
{
    internal interface IKrakenApi
    {
        [Post("/private/Balance")]
        Task<KrakenSchema.BalancesResponse> GetBalancesAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/private/DepositMethods")]
        Task<KrakenSchema.DepositMethodsResponse> GetDepositMethodsAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/private/DepositAddresses")]
        Task<KrakenSchema.DepositAddressesResponse> GetDepositAddresses([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Get("/public/AssetPairs")]
        Task<KrakenSchema.AssetPairsResponse> GetAssetPairsAsync();

        [Get("/public/Ticker?pair={pair}")]
        Task<KrakenSchema.TickersInformationResponse> GetTicketInformationAsync([Path] string pair);

        [Get("/public/OHLC?pair={pair}&interval={interval}")]
        Task<KrakenSchema.OhlcResponse> GetOhlcDataAsync([Path] string pair, [Path(Format = "D")] KrakenTimeInterval interval);

    }
}
