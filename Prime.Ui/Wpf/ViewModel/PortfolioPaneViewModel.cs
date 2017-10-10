using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using Prime.Core;
using Prime.Core.Wallet;
using GalaSoft.MvvmLight.Command;
using Humanizer;
using Prime.Core.Exchange.Rates;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class PortfolioPaneViewModel : DocumentPaneViewModel
    {
        public PortfolioPaneViewModel() { }

        public PortfolioPaneViewModel(ScreenViewModel screenViewModel)
        {
            _context = UserContext.Current;
            PCoordinator = _context.PortfolioCoordinator;
            ECoordinator = LatestPriceCoordinator.I;
            Dispatcher = Application.Current.Dispatcher;
            PCoordinator.Register<PortfolioChangedMessage>(this, PortfolioChanged);
            ECoordinator.Messenger.Register<LatestPriceResult>(this, ExchangeRateCollected);
        }


#pragma warning disable 169
        private readonly ScreenViewModel _screenViewModel;
#pragma warning restore 169
        public readonly Dispatcher Dispatcher;
        private readonly UserContext _context;
        public readonly PortfolioCoordinator PCoordinator;
        public readonly LatestPriceCoordinator ECoordinator;
        private DateTime _utcLastUpdated;
        private string _information;
        private Money _totalConverted;

        public event EventHandler OnChanged;

        public DateTime UtcLastUpdated
        {
            get => _utcLastUpdated;
            set => Set(ref _utcLastUpdated, value);
        }

        public Money TotalConverted
        {
            get => _totalConverted;
            set => Set(ref _totalConverted, value);
        }

        public string Information
        {
            get => _information;
            set => Set(ref _information, value);
        }

        public ObservableCollection<PortfolioGroupedItem> SummaryObservable { get; private set; } = new ObservableCollection<PortfolioGroupedItem>();

        public ObservableCollection<PortfolioLineItem> PortfolioObservable { get; private set; } = new ObservableCollection<PortfolioLineItem>();

        public ObservableCollection<PortfolioInfoItem> PortfolioInfoObservable { get; private set; } = new ObservableCollection<PortfolioInfoItem>();

        private void PortfolioChanged(PortfolioChangedMessage m)
        {
            try
            {
                PortfolioChangedCaught();
            }
            catch (Exception e)
            { }
        }

        private void PortfolioChangedCaught()
        {
            Dispatcher.Invoke(() =>
            {
                var q = UserContext.Current.QuoteAsset;
                var p = PCoordinator;

                PortfolioObservable.Clear();

                foreach (var i in p.Items.OrderBy(x => x.Asset?.ShortCode ?? "ZZZ").ThenBy(x => x.Network?.Name ?? "ZZZ"))
                    PortfolioObservable.Add(i);

                PortfolioInfoObservable.Clear();

                foreach (var i in p.PortfolioInfoItems.OrderBy(x => x.Network.NameLowered))
                    PortfolioInfoObservable.Add(i);

                var t = PortfolioInfoObservable.Select(x => x.TotalConvertedAssetValue).Sum();
                var r = t==0 ? 0 : 100 / t;
                foreach (var i in PortfolioInfoObservable)
                    i.Percentage = r * i.TotalConvertedAssetValue;

                var gi = new List<PortfolioGroupedItem>();
                foreach (var i in p.Items.GroupBy(x => x.Asset))
                    gi.Add(PortfolioGroupedItem.Create(q, i.Key, i.ToList()));

                SummaryObservable.Clear();
                foreach (var i in gi.OrderBy(x => x.IsTotalLine).ThenByDescending(x => x.Converted))
                    SummaryObservable.Add(i);

                UtcLastUpdated = p.UtcLastUpdated;
                Information = null;

                Information += "Via: " + string.Join(", ", p.WorkingProviders.OrderBy(x => x.Network.Name).Select(x => x.Network.Name));

                if (p.FailingProviders.Any())
                    Information += " | Failed: " + string.Join(", ", p.FailingProviders.OrderBy(x => x.Network.Name).Select(x => x.Network.Name));

                TotalConverted = p.Items.FirstOrDefault(x => x.IsTotalLine)?.Converted ?? Money.Zero;

                if (p.Items.Any(x=>x.ConversionFailed))
                    Information += " Warning: Some currencies could not be converted, and so are not included in this total.";

                OnChanged?.Invoke(this, EventArgs.Empty);

                RaisePropertyChanged(nameof(UtcLastUpdated));
                RaisePropertyChanged(nameof(TotalConverted));
                RaisePropertyChanged(nameof(Information));
                RaisePropertyChanged(nameof(SummaryObservable));
                RaisePropertyChanged(nameof(PortfolioObservable));
                RaisePropertyChanged(nameof(PortfolioInfoObservable));
            });

            DoExchangeRates();
        }

        private void DoExchangeRates()
        {
            var items = PCoordinator.Items.ToList();

            foreach (var i in items.Where(x=>!x.IsTotalLine && x.Asset!=null && !Equals(x.Asset, Asset.None)))
                ECoordinator.AddRequest(this, new AssetPair(i.Asset, UserContext.Current.QuoteAsset), i.Network);
        }

        private void ExchangeRateCollected(LatestPriceResult m)
        {
            try
            {
                var q = UserContext.Current.QuoteAsset;
                var items = PCoordinator.Items.ToList();
                var ex = items.FirstOrDefault(x => Equals(x.Asset, m.Pair.Asset1) && Equals(q, m.Pair.Asset2));
                if (ex == null)
                    return;
                ex.Converted = new Money(m.Price * ex.Total, q);
                ex.ConversionFailed = false;
            }
            catch{}
        }

        public override CommandContent Create()
        {
            return new SimpleContentCommand("portfolio");
        }

        public override void Dispose()
        {
            PCoordinator.Unregister<PortfolioChangedMessage>(this, PortfolioChanged);
            base.Dispose();
        }
    }
}
