using System;
using System.Threading.Tasks;
using Prime.Plugins.Services.Coinbase;
using RestEase;

namespace Prime.Plugins.Services.Gdax
{
    [Header("User-Agent", "RestEase")]
    internal interface IGdaxApi
    {
        [Get("/products")]
        Task<GdaxSchema.ProductsResponse> GetProductsAsync();

        [Get("/products/{productId}/book?level={level}")]
        Task<GdaxSchema.OrderBookResponse> GetProductOrderBookAsync([Path] string productId, [Path(Format = "D")] OrderBookDepthLevel level);

        [Get("/products/{curencyPair}/candles")]
        Task<decimal[][]> GetCandlesAsync(
            [Path] string curencyPair, 
            [Query("start", Format = "o")] DateTime? start = null, 
            [Query("end", Format = "o")] DateTime? end = null, 
            [Query("granularity")] int? granularity = null);

        /// <summary>
        /// Gets ticker for specified currency.
        /// See https://docs.gdax.com/#get-product-ticker.
        /// </summary>
        /// <param name="currency">Currency pair (separated by dash) which ticker is to be returned.</param>
        /// <returns>Ticker for specified currency.</returns>
        [Get("/products/{currency}/ticker")]
        Task<GdaxSchema.ProductTickerResponse> GetProductTickerAsync([Path] string currency);

        [Get("/time")]
        Task<GdaxSchema.TimeResponse> GetCurrentServerTimeAsync();
    }
}
