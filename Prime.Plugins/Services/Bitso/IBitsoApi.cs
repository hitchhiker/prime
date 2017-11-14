using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bitso
{
    internal interface IBitsoApi
    {
        [Get("/ticker/?book={currencyPair}")]
        Task<BitsoSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/available_books/")]
        Task<BitsoSchema.AvailableBooksResponse> GetAssetPairsAsync();
    }
}
