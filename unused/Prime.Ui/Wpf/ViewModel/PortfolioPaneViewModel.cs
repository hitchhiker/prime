using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private readonly TimerRegular _timer;
        private readonly PortfolioSubscribeMessage _subscription;

        public PortfolioPaneViewModel()
        {
            if (IsInDesignMode)
                return;

            M.Register<PortfolioResultsMessage>(this, UserContext.Current.Token, PortfolioChanged);

            _subscription = new PortfolioSubscribeMessage(Id, SubscriptionType.Subscribe);
            _timer = new TimerRegular(TimeSpan.FromSeconds(5), KeepSubscriptionAlive, true);
        }

        private void KeepSubscriptionAlive()
        {
            M.Send(_subscription, UserContext.Current.Token);
        }

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

        public BindingList<PortfolioGroupedItem> SummaryList { get; private set; } = new BindingList<PortfolioGroupedItem>();

        public BindingList<PortfolioLineItem> AssetsList { get; private set; } = new BindingList<PortfolioLineItem>();

        public BindingList<PortfolioNetworkInfoItem> NetworkList { get; private set; } = new BindingList<PortfolioNetworkInfoItem>();

        private void PortfolioChanged(PortfolioResultsMessage m)
        {
            try
            {
                PortfolioChangedCaught(m);
            }
            catch
            { }
        }

        private void PortfolioChangedCaught(PortfolioResultsMessage m)
        {
            UiDispatcher.Invoke(() =>
            {
                var info = m;
                var items = info.Items;

                AssetsList.Clear();

                foreach (var i in items.OrderBy(x => x.Asset?.ShortCode ?? "ZZZ").ThenBy(x => x.Network?.Name ?? "ZZZ"))
                    AssetsList.Add(i);

                NetworkList.Clear();

                foreach (var i in info.NetworkItems.OrderBy(x => x.Network.NameLowered))
                    NetworkList.Add(i);
                
                var gi = info.GroupedAsset;

                SummaryList.Clear();
                foreach (var i in gi.OrderBy(x => x.IsTotalLine).ThenByDescending(x => (decimal)x.Converted))
                    SummaryList.Add(i);

                var information = "Via: " + string.Join(", ", info.WorkingProviders.OrderBy(x => x.Network.Name).Select(x => x.Network.Name));

                if (info.FailingProviders.Any())
                    information += " | Failed: " + string.Join(", ", info.FailingProviders.OrderBy(x => x.Network.Name).Select(x => x.Network.Name));

                TotalConverted = items.FirstOrDefault(x => x.IsTotalLine)?.Converted ?? Money.Zero;

                if (items.Any(x=>x.ConversionFailed))
                    information += " Warning: Some currencies could not be converted, and so are not included in this total.";

                UtcLastUpdated = info.UtcLastUpdated;
                Information = information;
            });
        }

        public override CommandContent GetPageCommand()
        {
            return new SimpleContentCommand("portfolio");
        }

        public override void Dispose()
        {
            M.Unregister(this);
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
