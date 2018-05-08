using Prime.Common;

namespace Prime.Ui.Wpf.ViewModel.Ticker
{
    public class TickerPriceViewModel : TickerItemBaseViewModel
    {
        public TickerPriceViewModel(Asset asset, MarketPrice latestPrice)
        {
            Asset = asset;
            LatestPrice = latestPrice;
        }

        public Asset Asset { get; set; }

        private MarketPrice _latestPrice;
        public MarketPrice LatestPrice
        {
            get => _latestPrice;
            set => Set(ref _latestPrice, value);
        }

        public PriceStatistics Statistics => LatestPrice.PriceStatistics;
    }
}