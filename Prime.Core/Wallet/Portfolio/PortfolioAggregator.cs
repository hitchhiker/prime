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

        private Asset _quoteAsset;
        private readonly UniqueList<PortfolioLineItem> _items = new UniqueList<PortfolioLineItem>();
        private readonly UniqueList<PortfolioNetworkInfoItem> _infoItems= new UniqueList<PortfolioNetworkInfoItem>();
        private readonly List<PortfolioGroupedItem> _groupedAsset = new List<PortfolioGroupedItem>();

        private readonly List<IWalletService> _failingProviders = new List<IWalletService>();
        private readonly List<IWalletService> _workingProviders= new List<IWalletService>();
        private readonly List<IWalletService> _queryingProviders = new List<IWalletService>();
        private readonly List<IWalletService> _hasCollected = new List<IWalletService>();

        public ObjectId UserId => Context.Id;

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
                _quoteAsset = Context.QuoteAsset;
                var providers = Networks.I.WalletProviders.WithApi();
                _scanners.Clear();
                _scanners.AddRange(providers.Select(x => new PortfolioProvider(new PortfolioProviderContext(this.Context, x, _quoteAsset, TimerInterval))).ToList());
                _scanners.ForEach(x => x.OnChanged += ProviderChanged);
                _timer.Start();

                M.RegisterAsync<LatestPriceResultMessage>(this, IncomingLatestPrice);
                M.RegisterAsync<LatestPriceRequestMessage>(this, QuickFindPrice);
            }
        }

        private void QuickFindPrice(LatestPriceRequestMessage message)
        {
            if (message.Pair == null || message.SubscriptionType != SubscriptionType.Subscribe)
                return;

            var r = Core.Prime.I.LatestPriceAggregator.Results().FirstOrDefault(x => x.Pair.Equals(message.Pair));
            if (r!=null)
                M.SendAsync(r);
        }

        public void Stop()
        {
            lock (_lock)
            {
                _timer.Stop();
                _scanners.ForEach(delegate(PortfolioProvider x)
                {
                    x.OnChanged -= ProviderChanged;
                    x.Dispose();
                });
                _scanners.Clear();

                _items.Clear();
                _infoItems.Clear();
                _failingProviders.Clear();
                _workingProviders.Clear();
                _queryingProviders.Clear();
                UtcLastUpdated = DateTime.MinValue;

                M.SendAsync(new LatestPriceRequestMessage(_id, SubscriptionType.UnsubscribeAll));
                M.UnregisterAsync(this);

                SendChangedMessageDebounced();
            }
        }

        private void ProviderChanged(object sender, EventArgs e)
        {
            lock (_lock)
            {
                var ev = e as PortfolioProvider.PortfolioChangedLineEvent;
                var li = ev?.Item;
                var instance = sender as PortfolioProvider;

                if (!_hasCollected.Contains(instance.Provider))
                    _hasCollected.Add(instance.Provider);

                UpdateScanningStatuses();

                var prov = instance.Provider;
                UtcLastUpdated = instance.UtcLastUpdated;

                if (li != null)
                {
                    _items.RemoveAll(x => Equals(x.Network, prov.Network) && Equals(x.Asset, li.Asset));
                    _items.Add(li);
                }
                else
                {
                    _items.RemoveAll(x => Equals(x.Network, prov.Network));
                    foreach (var i in instance.Items)
                        _items.Add(i);
                }

                _infoItems.Add(instance.NetworkInfo, true);

                DoFinal();

                SendChangedMessageDebounced();
            }
        }


        private void UpdateScanningStatuses()
        {
            _workingProviders.Clear();
            _workingProviders.AddRange(_scanners.Where(x => x.IsConnected).Select(x => x.Provider));

            _failingProviders.Clear();
            _failingProviders.AddRange(_scanners.Where(x => x.IsFailing).Select(x => x.Provider));

            _queryingProviders.Clear();
            _queryingProviders.AddRange(_scanners.Where(x => x.IsQuerying).Select(x => x.Provider));

            _hasCollected.RemoveAll(x => _failingProviders.Contains(x));
        }

        private void SendChangedMessageDebounced()
        {
            _debouncer.Debounce(25, delegate
            {
                DoFinal();
                SendChangedMessage();
            });
        }

        public void SendChangedMessage()
        {
            lock (_lock)
            {
                var msg = new PortfolioResultsMessage
                {
                    UtcLastUpdated = UtcLastUpdated,
                    Items = _items.ToList(),
                    NetworkItems = _infoItems.ToList(),
                    GroupedAsset = _groupedAsset.ToList(),
                    WorkingProviders = _workingProviders.ToList(),
                    QueryingProviders = _queryingProviders.ToList(),
                    FailingProviders = _failingProviders.ToList(),
                    IsCollectionComplete = _scanners.All(x => _hasCollected.Contains(x.Provider)),
                    IsConversionComplete = _items.All(x => x.Converted != null),
                    TotalConverted = _items.Where(x => x != null && !x.IsTotalLine).Select(x => x.Converted).Sum(_quoteAsset)
                };

                M.Send(msg, Context.Token);
            }
        }

        private void DoFinal()
        {
            lock (_lock)
            {
                _items.RemoveAll(x => x.IsTotalLine);
                var qa = _quoteAsset;
                

                SubscribePriceConversion(qa);

                if (_items.Any())
                    _items.Add(new PortfolioTotalLineItem() { Items = _items });

                _groupedAsset.Clear();

                foreach (var i in _items.GroupBy(x => x.Asset))
                    _groupedAsset.Add(PortfolioGroupedItem.Create(qa, i.Key, i.ToList()));

                foreach (var n in _infoItems)
                    n.ConvertedTotal = _items.Where(x=>!x.IsTotalLine && Equals(x.Network, n.Network)).Select(x=>x.Converted).Sum(_quoteAsset);

                if (_infoItems.All(x => x.ConvertedTotal != null))
                {
                    var t = _infoItems.Select(x => x.ConvertedTotal).Sum();
                    var r = t == 0 ? 0 : 100 / t;

                    foreach (var n in _infoItems)
                        n.Percentage = Math.Round(n.ConvertedTotal.Value.ToDecimalValue() * r, 2);
                }
            }
        }

        private void SubscribePriceConversion(Asset qa)
        {
            var subscribed = new List<AssetPair>();

            foreach (var i in _items)
            {
                if (Equals(i.Asset, qa))
                {
                    i.Converted = i.Total;
                    continue;
                }

                var pair = new AssetPair(i.Asset, qa);

                if (subscribed.Contains(pair))
                    continue;

                subscribed.Add(pair);
                M.SendAsync(new LatestPriceRequestMessage(_id, pair));
            }
        }

        private void IncomingLatestPrice(LatestPriceResultMessage m)
        {
            lock (_lock)
            {
                var qa = UserContext.Current.QuoteAsset;
                var ti = _items.Where(x => Equals(x.Total.Asset, m.Pair.Asset1) && Equals(qa, m.Pair.Asset2));

                if (DoConversion(ti, m, qa))
                    SendChangedMessageDebounced();
            }
        }

        private static bool DoConversion(IEnumerable<PortfolioLineItem> items, LatestPriceResultMessage m, Asset targetAsset)
        {
            var change = false;
            foreach (var i in items)
            {
                var mn = new Money((decimal)i.Total * (decimal)m.Price, targetAsset);
                if (i.Converted != null && mn.ToDecimalValue() == i.Converted.Value.ToDecimalValue())
                    continue;

                i.Converted = mn;
                i.ConversionFailed = false;
                change = true;
            }
            return change;
        }
    }
}