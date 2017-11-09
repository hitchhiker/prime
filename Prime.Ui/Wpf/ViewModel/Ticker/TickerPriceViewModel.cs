using Prime.Common;

namespace Prime.Ui.Wpf.ViewModel
{
    public class TickerPriceViewModel : TickerItemBaseViewModel
    {
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