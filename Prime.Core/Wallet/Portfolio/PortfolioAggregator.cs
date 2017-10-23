using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using LiteDB;
using Nito.AsyncEx;
using Prime.Common.Exchange.Rates;
using Prime.Utility;

namespace Prime.Common.Wallet
{
    internal class PortfolioAggregator
    {
        public PortfolioAggregator(UserContext context)
        {
            _id = ObjectId.NewObjectId();
            TimerInterval = 15000;
            Context = context;
            _scanners = new List<PortfolioProvider>();
            M.Register<QuoteAssetChangedMessage>(this, BaseAssetChanged);
            _debouncer = new Debouncer();
            _timer = new TimerIrregular(TimeSpan.FromSeconds(30), KeepPricesSubscriptionAlive);
            _keepAlive = new LatestPriceRequestMessage(_id);
        }

        private readonly ObjectId _id;
        private readonly TimerIrregular _timer;
        public readonly int TimerInterval;
        protected readonly IMessenger M = DefaultMessenger.I.Default;
        private readonly Debouncer _debouncer;
        private readonly List<PortfolioProvider> _scanners;
        public readonly UserContext Context;
        private readonly object _lock = new object();
        private readonly LatestPriceRequestMessage _keepAlive;

        public UniqueList<PortfolioLineItem> Items { get; } = new UniqueList<PortfolioLineItem>();
        public List<IWalletService> FailingProviders { get; } = new List<IWalletService>();
        public List<IWalletService> WorkingProviders { get; } = new List<IWalletService>();
        public List<IWalletService> QueryingProviders { get; } = new List<IWalletService>();
        public UniqueList<PortfolioInfoItem> PortfolioInfoItems { get; } = new UniqueList<PortfolioInfoItem>();
        public DateTime UtcLastUpdated { get; private set; }

        private void KeepPricesSubscriptionAlive()
        {
            M.Send(_keepAlive);
        }

        private void BaseAssetChanged(QuoteAssetChangedMessage m)
        {
            lock (_lock)
            {
                Stop();
                Start();
            }
        }

        public void Start()
        {
            lock (_lock)
            {
                var providers = Networks.I.WalletProviders.WithApi();
                _scanners.Clear();
                _scanners.AddRange(providers.Select(x => new PortfolioProvider(new PortfolioProviderContext(this.Context, x, Context.QuoteAsset, TimerInterval))).ToList());
                _scanners.ForEach(x => x.OnChanged += ScannerChanged);
                _timer.Start();

                M.Register<LatestPriceResultMessage>(this, IncomingLatestPrice);
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                _timer.Stop();
                _scanners.ForEach(delegate(PortfolioProvider x)
                {
                    x.OnChanged -= ScannerChanged;
                    x.Dispose();
                });
                _scanners.Clear();

                Items.Clear();
                PortfolioInfoItems.Clear();
                FailingProviders.Clear();
                WorkingProviders.Clear();
                QueryingProviders.Clear();
                UtcLastUpdated = DateTime.MinValue;

                M.Send(new LatestPriceRequestMessage(_id, SubscriptionType.UnsubscribeAll));
                M.Unregister(this);

                SendChangedMessageDebounced();
            }
        }

        private void ScannerChanged(object sender, EventArgs e)
        {
            lock (_lock)
            {
                var ev = e as PortfolioProvider.PortfolioChangedLineEvent;
                var li = ev?.Item;
                var scanner = sender as PortfolioProvider;

                UpdateScanningStatuses();

                var prov = scanner.Provider;
                UtcLastUpdated = scanner.UtcLastUpdated;

                if (li != null)
                {
                    Items.RemoveAll(x => Equals(x.Network, prov.Network) && Equals(x.Asset, li.Asset));
                    Items.Add(li);
                }
                else
                {
                    Items.RemoveAll(x => Equals(x.Network, prov.Network));
                    foreach (var i in scanner.Items)
                        Items.Add(i);
                }

                PortfolioInfoItems.Add(scanner.Info, true);

                DoFinal();

                SendChangedMessageDebounced();
            }
        }

        private void SendChangedMessageDebounced()
        {
            _debouncer.Debounce(5, _ => SendChangedMessage());
        }

        public void SendChangedMessage()
        {
            var msg = new PortfolioChangedMessage(Context.Id, UtcLastUpdated, Items.ToUniqueList(), PortfolioInfoItems.ToUniqueList(), WorkingProviders.ToList(), QueryingProviders.ToList(), FailingProviders.ToList());
            M.Send(msg);
        }

        private void DoFinal()
        {
            Items.RemoveAll(x => x.IsTotalLine);
            
            var subscribed = new List<AssetPair>();

            foreach (var i in Items)
            {
                if (Equals(i.Asset, UserContext.Current.QuoteAsset))
                {
                    i.Converted = i.Total;
                    continue;
                }

                var pair = new AssetPair(i.Asset, UserContext.Current.QuoteAsset);

                if (subscribed.Contains(pair))
                    continue;

                subscribed.Add(pair);
                M.Send(new LatestPriceRequestMessage(_id, pair));
            }

            if (Items.Any())
                Items.Add(new PortfolioTotalLineItem() { Items = Items });
        }


        private void IncomingLatestPrice(LatestPriceResultMessage m)
        {
            lock (_lock)
            {
                var qa = UserContext.Current.QuoteAsset;
                var ti = Items.FirstOrDefault(x => Equals(x.Total.Asset, m.Pair.Asset1) && Equals(qa, m.Pair.Asset2));
                if (ti == null)
                    return;

                if (DoConversion(ti, m, qa))
                    SendChangedMessageDebounced();
            }
        }

        private static bool DoConversion(PortfolioLineItem i, LatestPriceResultMessage m, Asset targetAsset)
        {
            var mn = new Money((decimal)i.Total * (decimal)m.Price, targetAsset);
            if (i.Converted != null && mn.ToDecimalValue() == i.Converted.Value.ToDecimalValue())
                return false;

            i.Converted = mn;
            i.ConversionFailed = false;
            return true;
        }

        private void UpdateScanningStatuses()
        {
            WorkingProviders.Clear();
            WorkingProviders.AddRange(_scanners.Where(x => x.IsConnected).Select(x => x.Provider));

            FailingProviders.Clear();
            FailingProviders.AddRange(_scanners.Where(x => x.IsFailing).Select(x => x.Provider));

            QueryingProviders.Clear();
            QueryingProviders.AddRange(_scanners.Where(x => x.IsQuerying).Select(x => x.Provider));
        }
    }
}