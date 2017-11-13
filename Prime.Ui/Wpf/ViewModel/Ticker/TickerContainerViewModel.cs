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
            tickerTop.ListTickerItems.Add(new TickerNewsItemViewModel("Realy Long Item", "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo."));
            tickerTop.ListTickerItems.Add(new TickerNewsItemViewModel("Bitcoin Up 20%", "More stuff about bitcoin"));
            tickerTop.ListTickerItems.Add(new TickerNewsItemViewModel("Ethereum Up 10%", "More stuff about ETH"));

            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel(Asset.Btc, new MarketPrice(null,Asset.Btc, 6000)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel(Asset.Usd, new MarketPrice(null,Asset.Usd, 500)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel(Asset.Eur, new MarketPrice(null,Asset.Eur, 700)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel(Asset.Eth, new MarketPrice(null,Asset.Eth, 200)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel(Asset.Jpy, new MarketPrice(null,Asset.Jpy, 100)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel(Asset.Krw, new MarketPrice(null,Asset.Krw, 500)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel(Asset.Xrp, new MarketPrice(null,Asset.Xrp, 600)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel("CANN".ToAssetRaw(), new MarketPrice(null, "CANN".ToAssetPairRaw(), (decimal)100.3)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel("WAVES".ToAssetRaw(), new MarketPrice(null, "WAVES".ToAssetPairRaw(), (decimal)230.4)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel("BTH".ToAssetRaw(), new MarketPrice(null, "BTH".ToAssetPairRaw(), (decimal)530.45)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel("BTG".ToAssetRaw(), new MarketPrice(null, "BTG".ToAssetPairRaw(), (decimal)2342.23)));
            tickerBottom.ListTickerItems.Add(new TickerPriceViewModel("NEO".ToAssetRaw(), new MarketPrice(null, "NEO".ToAssetPairRaw(), (decimal)342.753)));

            ListTickers.Add(tickerTop);
            ListTickers.Add(tickerBottom);
        }

        public ObservableCollection<TickerViewModel> ListTickers { get; } = new ObservableCollection<TickerViewModel>();
    }
}