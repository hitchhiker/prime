using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using Prime.Common;
using Prime.Common.Wallet;
using GalaSoft.MvvmLight.Command;
using Humanizer;
using Prime.Common.Exchange.Rates;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class PortfolioPaneViewModel : DocumentPaneViewModel
    {
        private DateTime _utcLastUpdated;
        private string _information;
        private Money _totalConverted;
        private readonly TimerIrregular _timer;

        public PortfolioPaneViewModel()
        {
            if (IsInDesignMode)
                return;

            M.Register<PortfolioChangedMessage>(this, PortfolioChanged);
            _timer = new TimerIrregular(TimeSpan.FromSeconds(30), KeepSubscriptionAlive);
            _timer.Start();
        }

        private void KeepSubscriptionAlive()
        {
            M.Send(new PortfolioSubscribeMessage(Id, UserContext.Current.Id));
        }

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
                PortfolioChangedCaught(m);
            }
            catch
            { }
        }

        private void PortfolioChangedCaught(PortfolioChangedMessage m)
        {
            if (m.UserId != UserContext.Current.Id)
                return;

            UiDispatcher.Invoke(() =>
            {
                var q = UserContext.Current.QuoteAsset;
                var items = m.Items;

                PortfolioObservable.Clear();

                foreach (var i in items.OrderBy(x => x.Asset?.ShortCode ?? "ZZZ").ThenBy(x => x.Network?.Name ?? "ZZZ"))
                    PortfolioObservable.Add(i);

                PortfolioInfoObservable.Clear();

                foreach (var i in m.InfoItems.OrderBy(x => x.Network.NameLowered))
                    PortfolioInfoObservable.Add(i);

                var t = PortfolioInfoObservable.Select(x => x.TotalConvertedAssetValue).Sum();
                var r = t==0 ? 0 : 100 / t;
                foreach (var i in PortfolioInfoObservable)
                    i.Percentage = r * i.TotalConvertedAssetValue;

                var gi = new List<PortfolioGroupedItem>();
                foreach (var i in items.GroupBy(x => x.Asset))
                    gi.Add(PortfolioGroupedItem.Create(q, i.Key, i.ToList()));

                SummaryObservable.Clear();
                foreach (var i in gi.OrderBy(x => x.IsTotalLine).ThenByDescending(x => (decimal)x.Converted))
                    SummaryObservable.Add(i);

                UtcLastUpdated = m.UtcLastUpdated;
                Information = null;

                Information += "Via: " + string.Join(", ", m.Working.OrderBy(x => x.Network.Name).Select(x => x.Network.Name));

                if (m.Failing.Any())
                    Information += " | Failed: " + string.Join(", ", m.Failing.OrderBy(x => x.Network.Name).Select(x => x.Network.Name));

                TotalConverted = items.FirstOrDefault(x => x.IsTotalLine)?.Converted ?? Money.Zero;

                if (items.Any(x=>x.ConversionFailed))
                    Information += " Warning: Some currencies could not be converted, and so are not included in this total.";

                OnChanged?.Invoke(this, EventArgs.Empty);

                RaisePropertyChanged(nameof(UtcLastUpdated));
                RaisePropertyChanged(nameof(TotalConverted));
                RaisePropertyChanged(nameof(Information));
                RaisePropertyChanged(nameof(SummaryObservable));
                RaisePropertyChanged(nameof(PortfolioObservable));
                RaisePropertyChanged(nameof(PortfolioInfoObservable));
            });
        }

        public override CommandContent GetPageCommand()
        {
            return new SimpleContentCommand("portfolio");
        }

        public override void Dispose()
        {
            M.Send(new PortfolioSubscribeMessage(Id, SubscriptionType.UnsubscribeAll));
            M.Unregister(this);
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
