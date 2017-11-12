using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Ui.Wpf.ViewModel.Ticker
{
    public class TickerContainerViewModel : VmBase
    {
        public TickerContainerViewModel()
        {
            Task.Run((Action)Populate);
        }

        public void Populate()
        {
            var tickerTop = new TickerViewModel();
            var tickerBottom = new TickerViewModel();

            tickerTop.ListTickerItems.Add(new TickerNewsItemViewModel("Breaking", "This is a breaking story"));
            tickerTop.ListTickerItems.Add(new TickerNewsItemViewModel("News Item", "This is a news item"));
            tickerTop.ListTickerItems.Add(new TickerNewsItemViewModel("News Item", "This is another news item"));

            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel(Asset.Btc, new MarketPrice(Asset.Btc, 6000)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel(Asset.Usd, new MarketPrice(Asset.Usd, 500)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel(Asset.Eur, new MarketPrice(Asset.Eur, 700)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel(Asset.Eth, new MarketPrice(Asset.Eth, 200)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel(Asset.Jpy, new MarketPrice(Asset.Jpy, 100)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel(Asset.Krw, new MarketPrice(Asset.Krw, 500)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel(Asset.Xrp, new MarketPrice(Asset.Xrp, 600)));

            ListTickers.Add(tickerTop);
            ListTickers.Add(tickerBottom);
        }

        public ObservableCollection<TickerViewModel> ListTickers { get; } = new ObservableCollection<TickerViewModel>();
    }
}