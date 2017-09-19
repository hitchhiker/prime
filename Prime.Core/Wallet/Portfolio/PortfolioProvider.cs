using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using GalaSoft.MvvmLight.Messaging;
using Nito.AsyncEx;
using Prime.Utility;

namespace Prime.Core.Wallet
{

    public class PortfolioProvider
    {
        public PortfolioProvider(UserContext context)
        {
            TimerFrequency = 15000;
            Context = context;
            _scanners = new List<PortfolioProviderScanner>();
            StartScanners(Context);
            _messenger.Register<BaseAssetChangedMessage>(this, BaseAssetChanged);
        }

        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        private readonly List<PortfolioProviderScanner> _scanners;
        public readonly UserContext Context;
        private readonly object _lock = new object();
        protected int TimerFrequency { get; set; }

        public UniqueList<PortfolioLineItem> Items { get; } = new UniqueList<PortfolioLineItem>();
        public List<IWalletService> FailingProviders { get; } = new List<IWalletService>();
        public List<IWalletService> WorkingProviders { get; } = new List<IWalletService>();
        public List<IWalletService> QueryingProviders { get; } = new List<IWalletService>();
        public UniqueList<PortfolioInfoItem> PortfolioInfoItems { get; } = new UniqueList<PortfolioInfoItem>();
        public DateTime UtcLastUpdated { get; private set; }

        public int RegisteredCount { get; private set; } = 0;
        public bool IsScanning { get; private set; }

        public void Register(object subscriber, Action<PortfolioChangedMessage> action)
        {
            RegisteredCount++;
            _messenger.Register(subscriber, action);
            new Task(RegistrationChanged).Start();
        }

        public void Unregister(object subscriber, Action<PortfolioChangedMessage> action)
        {
            RegisteredCount--;
            _messenger.Unregister(subscriber, action);
            new Task(RegistrationChanged).Start();
        }

        private void RegistrationChanged()
        {
            lock (_lock)
            {
                if (RegisteredCount < 1)
                {
                    RegisteredCount = 0;
                    StopScanners();
                }
                else
                {
                    StartScanners(Context);
                }
            }
        }

        private void BaseAssetChanged(BaseAssetChangedMessage m)
        {
            lock (_lock)
            {
                StopScanners();
                StartScanners(Context);
            }
        }

        private void StartScanners(UserContext context)
        {
            lock (_lock)
            {
                if (TimerFrequency == 0 || IsScanning)
                    return;

                var providers = Networks.I.WalletProviders;
                _scanners.Clear();
                _scanners.AddRange(providers.Select(x => new PortfolioProviderScanner(new PortfolioProviderScannerContext(this.Context, x, context.BaseAsset, TimerFrequency))).ToList());
                _scanners.ForEach(x => x.OnChanged += ScannerChanged);

                IsScanning = true;
            }
        }

        private void StopScanners()
        {
            lock (_lock)
            {
                if (!IsScanning)
                    return;

                _scanners.ForEach(delegate(PortfolioProviderScanner x)
                {
                    x.OnChanged -= ScannerChanged;
                    x.Dispose();
                });

                Items.Clear();
                PortfolioInfoItems.Clear();
                FailingProviders.Clear();
                WorkingProviders.Clear();
                QueryingProviders.Clear();
                UtcLastUpdated = DateTime.MinValue;
                IsScanning = false;
                _messenger.Send(new PortfolioChangedMessage());
            }
        }

        private void ScannerChanged(object sender, EventArgs e)
        {
            lock (_lock)
            {
                var ev = e as PortfolioProviderScanner.PortfolioChangedLineEvent;
                var li = ev?.Item;
                var scanner = sender as PortfolioProviderScanner;

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

                DoTotal(scanner.BaseAsset);

                _messenger.Send(new PortfolioChangedMessage());
            }
        }

        private void DoTotal(Asset bAsset)
        {
            Items.RemoveAll(x => x.IsTotalLine);
            if (Items.Any())
                Items.Add(new PortfolioLineItem() { IsTotalLine = true, Converted = new Money(this.Items.Sum(x=>(decimal)x.Converted), bAsset) });
        }
        
        private void UpdateScanningStatuses()
        {
            lock (_lock)
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
}